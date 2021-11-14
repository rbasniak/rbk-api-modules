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
    [Route("api/code-generator")]
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
        public async Task<ActionResult> Get(bool directUpdate, string projectId)
        {
            String basePath;
            if (directUpdate)
            {
                var searchString = $"frontend\\src\\app\\auto-generated*";

                if (!String.IsNullOrEmpty(projectId))
                {
                    searchString = $"frontend\\{projectId}\\src\\app\\auto-generated*"; ;
                }

                var applicationPath = new DirectoryInfo(_environment.ContentRootPath);
                var codePath = Directory.GetDirectories(applicationPath.Parent.Parent.FullName, searchString, SearchOption.AllDirectories);

                if (codePath.Length != 1)
                {
                    return Ok("Não foi possivel localizar o repositírio");
                }

                try
                {
                    var files = Directory.GetFiles(codePath.First(), "*.*", SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        if (Path.GetExtension(file).ToLower() != ".ts")
                        {
                            return Ok("Abortando geração por medo de estar no diretório errado: " + codePath.First());
                        }

                        System.IO.File.Delete(file);
                    }
                }
                catch (Exception ex)
                {
                    return Ok("Não foi possível apagar os arquivos no repositório");
                }

                basePath = codePath.First(); 
            }
            else
            {
                basePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "_temp", Guid.NewGuid().ToString());
            }
            
            if (directUpdate)
            {
                var codeGenerator = new AngularCodeGenerator(projectId, basePath);
                var data = codeGenerator.GetData();

                return Ok("Arquivos atualizados com sucesso :)");
            }
            else
            {
                var codeGenerator = new AngularCodeGenerator(projectId, Path.Combine(basePath, "code"));
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
}
