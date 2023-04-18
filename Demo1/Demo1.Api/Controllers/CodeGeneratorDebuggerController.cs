using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core.CodeGeneration;
using rbkApiModules.Commons.Core.Localization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Demo1.Api.Controllers
{
    [IgnoreOnCodeGeneration]
    [ApiController]
    [Route("api/code-generator-debugger")]
    public class CodeGeneratorDebuggerController: ControllerBase
    {
        [HttpGet]
        public IActionResult TestCodeGenerator([FromServices] IWebHostEnvironment environment, [FromServices] ILocalizationService localization, string path, string applicationName)
        {
            var outputPath = environment.WebRootPath;

            var files = Directory.GetFiles(path);

            var assemblies = files.Where(x => Path.GetFileName(x).ToLower().Contains(applicationName) && Path.GetFileName(x).ToLower().EndsWith(".dll")).ToArray();

            var codeGenerator = new AngularCodeGenerator(null, Path.Combine(environment.WebRootPath, "_code-generator"), localization);

            codeGenerator.Generate(assemblies.Select(x => Assembly.LoadFrom(x)).ToArray());

            return Ok("Done!");
        }
    }
}
