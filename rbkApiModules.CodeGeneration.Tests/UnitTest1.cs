using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace rbkApiModules.CodeGeneration.Tests
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            var assemblies = new[] {
                //Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MemoreX.Models.dll"),
                //Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MemoreX.Database.dll"),
                //Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MemoreX.BusinessLogic.dll"),
                //Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MemoreX.Services.dll"),
                //Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MemoreX.DataTransfer.dll"),
                //Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MemoreX.Api.dll"),

                Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MedicalHorizon.Models.dll"),
                Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MedicalHorizon.Database.dll"),
                Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MedicalHorizon.BusinessLogic.dll"),
                Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MedicalHorizon.Services.dll"),
                Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MedicalHorizon.DataTransfer.dll"),
                Assembly.LoadFrom(@"D:\Repositories\horizonte\medical-horizon\backend\MedicalHorizon.Api\bin\Debug\net5.0\MedicalHorizon.Api.dll"),
            };

            var codeGenerator = new AngularCodeGenerator(null, @"D:\Repositories\horizonte\medical-horizon\frontend\professional\src\app\auto-generated");
            var data = codeGenerator.GetData();
        }
    }
}
