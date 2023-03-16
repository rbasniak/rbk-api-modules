using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CodeGeneration;
using Serilog;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace Demo1.Api.Controllers
{
    [ApiController]
    [Route("api/code-generator-debugger")]
    public class CodeGeneratorDebuggerController: ControllerBase
    {
        [HttpGet]
        public IActionResult TestCodeGenerator([FromServices] IWebHostEnvironment environment, string path, string applicationName)
        {
            var outputPath = environment.WebRootPath;

            var files = Directory.GetFiles(path);

            var assemblies = files.Where(x => Path.GetFileName(x).ToLower().Contains(applicationName) && Path.GetFileName(x).ToLower().EndsWith(".dll")).ToArray();

            var codeGenerator = new AngularCodeGenerator(null, Path.Combine(environment.WebRootPath, "_code-generator"));

            codeGenerator.Generate(assemblies.Select(x => Assembly.LoadFrom(x)).ToArray());

            return Ok("Done!");
        }
    }
}
