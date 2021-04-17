using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.CodeGeneration.Commons
{
    [Route("api/code-geneerator")]
    [ApiController]
    [IgnoreOnCodeGeneration]
    [AllowAnonymous]
    public class CodeGeneratorController : BaseController
    {
        private readonly IWebHostEnvironment _environment;

        public CodeGeneratorController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var basePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "_temp", Guid.NewGuid().ToString());

            var codeGenerator = new AngularCodeGenerator(Path.Combine(basePath, "code"));
            var data = codeGenerator.GetData();

            var zipFile = Path.Combine(basePath, "code.zip");
            ZipFile.CreateFromDirectory(Path.Combine(basePath, "code"), zipFile);

            var memory = new MemoryStream();
            using (var stream = new FileStream(zipFile, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            Directory.Delete(basePath, true);

            return File(memory, "application/zip", "code.zip");
        }
    }
}
