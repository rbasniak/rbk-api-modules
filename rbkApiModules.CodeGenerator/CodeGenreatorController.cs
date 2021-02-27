//using Microsoft.AspNetCore.Mvc;
//using rbkApiModules.Infrastructure.Api;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace rbkApiModules.CodeGenerator
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CodeGenreatorController: BaseController
//    {
//        [HttpGet]
//        public ActionResult Get()
//        {
//            var codeGenerator = new AngularCodeGenerator();
//            return Ok(codeGenerator.GetControllers());
//        }
//    }

//    public class AngularCodeGenerator
//    {
//        public string GetControllers()
//        {
//            var result = new StringBuilder();

//            AppDomain currentDomain = AppDomain.CurrentDomain;
//            var assemblies = currentDomain.GetAssemblies();

//            foreach (var assembly in assemblies)
//            {
//                result.AppendLine(assembly.FullName);
//            }

//            return result.ToString();
//        }
//    }
//}
