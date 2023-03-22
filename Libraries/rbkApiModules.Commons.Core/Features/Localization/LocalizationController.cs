using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CodeGeneration;
using rbkApiModules.Commons.Core.Utilities.Localization;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Comments.Core.Controllers;

[IgnoreOnCodeGeneration]
[ApiController]
[Route("api/[controller]")]
public class LocalizationController : BaseController
{
    [HttpGet("test")]
    public ActionResult<string> Test()
    {
        var value = SharedValidationMessages.Common.FieldMustHaveLengthBetweenMinAndMax;

        var temp = value.GetType().FullName;

        return $"{value.GetType().FullName.Split('.').Last().Replace("+", "::")}::{value.ToString()}";

        // Carregar o dicionario básico, com todos enums e descriptions
        // Procurar todos os arquivos com extensao .localization
        // sobrescrever os dicionarios (ter language no nome do arquivo)
        // o dicionario será um dicionario dentro de um dicionario, 1o nivel sao os idiomas
        // o LocalizationService vai ter :
        //   - um idioma padrao
        //   - um método pra setar o idioma
        //   - tentara procurar o idioma nos headers http
        //   - olhara para os options da lib caso nao seja dinamico
    }

    [HttpGet("localization-template")]
    public ActionResult<string> GetLocalizationTemplate()
    {
        var results = new List<string>();

        AppDomain currentDomain = AppDomain.CurrentDomain;
        var assemblies = currentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var localizedResourceTypes = assembly.GetTypes()
                    .Where(x => typeof(ILocalizedResource).IsAssignableFrom(x) && !x.IsInterface);

            foreach (var localizedResourceType in localizedResourceTypes)
            {
                var localizedResourceChildTypes = localizedResourceType.GetNestedTypes();

                foreach (var localizedResourceChildType in localizedResourceChildTypes)
                {
                    if (localizedResourceChildType.IsEnum)
                    {
                        var enumValues = Enum.GetValues(localizedResourceChildType);

                        foreach (var enumValue in enumValues)
                        {
                            var enumType = localizedResourceChildType;
                            var memberInfos = enumType.GetMember(((Enum)enumValue).ToString());
                            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                            var message = "*** Missing localized message ***";

                            if (valueAttributes != null && valueAttributes.Length > 0)
                            {
                                message = ((DescriptionAttribute)valueAttributes[0]).Description;
                            }

                            results.Add($"{localizedResourceType.Name}::{localizedResourceChildType.Name}::{enumValue.ToString()}={message}");
                        }
                    }
                    else
                    {
                        var localizedResourceGrandChildTypes = localizedResourceChildType.GetNestedTypes();

                        foreach (var localizedResourceGrandChildType in localizedResourceGrandChildTypes)
                        {
                            if (localizedResourceGrandChildType.IsEnum)
                            {
                                var enumValues = Enum.GetValues(localizedResourceGrandChildType);

                                foreach (var enumValue in enumValues)
                                {
                                    var enumType = localizedResourceGrandChildType;
                                    var memberInfos = enumType.GetMember(((Enum)enumValue).ToString());
                                    var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                                    var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                                    var message = "*** Missing localized message ***";

                                    if (valueAttributes != null && valueAttributes.Length > 0)
                                    {
                                        message = ((DescriptionAttribute)valueAttributes[0]).Description;
                                    }

                                    results.Add($"{localizedResourceType.Name}::{localizedResourceChildType.Name}::{localizedResourceGrandChildType.Name}::{enumValue.ToString()}={message}");
                                }
                            }
                            else
                            {
                                throw new SafeException("Localization services does not support classes nested more than 2 levels deep");
                            }
                        }
                    }
                }
            }
        }

        return String.Join(Environment.NewLine, results);
    }
}
