using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core.CodeGeneration;
using rbkApiModules.Commons.Core.Utilities.Localization;
using Microsoft.AspNetCore.Authorization;

namespace rbkApiModules.Commons.Core.Localization;

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
        return _localizationService.LocalizeString(AuthenticationMessages.Erros.CannotDeleteUsedTenant);

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
