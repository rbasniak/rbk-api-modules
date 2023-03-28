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
using rbkApiModules.Commons.Core.Localization;
using Microsoft.AspNetCore.Authorization;

namespace rbkApiModules.Comments.Core.Controllers;

[IgnoreOnCodeGeneration]
[ApiController]
[Route("api/[controller]")]
public class LocalizationController : BaseController
{
    private readonly ILocalizationService _localizationService;

    public LocalizationController(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    [AllowAnonymous]
    [HttpGet("test")]
    public ActionResult<string> Test()
    {
        return _localizationService.GetValue(AuthenticationMessages.Erros.CannotDeleteUsedTenant);

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

    [AllowAnonymous]
    [HttpGet("localization-template")]
    public ActionResult<string> GetLocalizationTemplate(string localization = null)
    {
        return _localizationService.GetLanguageTemplate(localization);
    }
}
