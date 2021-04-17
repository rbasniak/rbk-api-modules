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
                Assembly.LoadFrom(@"D:\Repositories\pessoal\memorex2\backend\MemoreX.Api\bin\Debug\net5.0\MemoreX.Models.dll"),
                Assembly.LoadFrom(@"D:\Repositories\pessoal\memorex2\backend\MemoreX.Api\bin\Debug\net5.0\MemoreX.Database.dll"),
                Assembly.LoadFrom(@"D:\Repositories\pessoal\memorex2\backend\MemoreX.Api\bin\Debug\net5.0\MemoreX.BusinessLogic.dll"),
                Assembly.LoadFrom(@"D:\Repositories\pessoal\memorex2\backend\MemoreX.Api\bin\Debug\net5.0\MemoreX.Services.dll"),
                Assembly.LoadFrom(@"D:\Repositories\pessoal\memorex2\backend\MemoreX.Api\bin\Debug\net5.0\MemoreX.DataTransfer.dll"),
                Assembly.LoadFrom(@"D:\Repositories\pessoal\memorex2\backend\MemoreX.Api\bin\Debug\net5.0\MemoreX.Api.dll"),
            };

            var codeGenerator = new AngularCodeGenerator(Path.Combine("D:\\_temp", "code"));
            var data = codeGenerator.GetData();
        }
    }
}
