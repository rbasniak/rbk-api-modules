using System.Reflection;
using Xunit;

namespace rbkApiModules.CodeGeneration.Tests
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            //var assemblies = new[] {
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\proteus5\back\ProteusCoreApi.Api\bin\Debug\net6.0\ProteusCoreApi.Models.dll"),
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\proteus5\back\ProteusCoreApi.Api\bin\Debug\net6.0\ProteusCoreApi.Database.dll"),
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\proteus5\back\ProteusCoreApi.Api\bin\Debug\net6.0\ProteusCoreApi.BusinessLogic.dll"),
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\proteus5\back\ProteusCoreApi.Api\bin\Debug\net6.0\ProteusCoreApi.Services.dll"),
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\proteus5\back\ProteusCoreApi.Api\bin\Debug\net6.0\ProteusCoreApi.DataTransfer.dll"),
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\proteus5\back\ProteusCoreApi.Api\bin\Debug\net6.0\ProteusCoreApi.Api.dll"),
            //};
            //var codeGenerator = new AngularCodeGenerator(null, @"D:\Repositories\Tecgraf\proteus5\front\models\src\app\auto-generated");

            //var assemblies = new[] {
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\giants\back\Giants.Api\bin\Debug\net6.0\Giants.Models.dll"),
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\giants\back\Giants.Api\bin\Debug\net6.0\Giants.Database.dll"),
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\giants\back\Giants.Api\bin\Debug\net6.0\Giants.BusinessLogic.dll"),
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\giants\back\Giants.Api\bin\Debug\net6.0\Giants.Services.dll"),
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\giants\back\Giants.Api\bin\Debug\net6.0\Giants.DataTransfer.dll"),
            //    Assembly.LoadFrom(@"D:\Repositories\Tecgraf\giants\back\Giants.Api\bin\Debug\net6.0\Giants.Api.dll"),
            //};
            //var codeGenerator = new AngularCodeGenerator(null, @"D:\Repositories\Tecgraf\giants\front\src\app\auto-generated");

            var assemblies = new[] {
                Assembly.LoadFrom(@"D:\Repositories\Tecgraf\fapeng\web\back\fapeng.Api\bin\Debug\net6.0\fapeng.Models.dll"),
                Assembly.LoadFrom(@"D:\Repositories\Tecgraf\fapeng\web\back\fapeng.Api\bin\Debug\net6.0\fapeng.Database.dll"),
                Assembly.LoadFrom(@"D:\Repositories\Tecgraf\fapeng\web\back\fapeng.Api\bin\Debug\net6.0\fapeng.BusinessLogic.dll"),
                Assembly.LoadFrom(@"D:\Repositories\Tecgraf\fapeng\web\back\fapeng.Api\bin\Debug\net6.0\fapeng.Services.dll"),
                Assembly.LoadFrom(@"D:\Repositories\Tecgraf\fapeng\web\back\fapeng.Api\bin\Debug\net6.0\fapeng.DataTransfer.dll"),
                Assembly.LoadFrom(@"D:\Repositories\Tecgraf\fapeng\web\back\fapeng.Api\bin\Debug\net6.0\fapeng.Api.dll"),
            };
            var codeGenerator = new AngularCodeGenerator(null, @"D:\Repositories\Tecgraf\fapeng\web\front\web\src\app\auto-generated");

            codeGenerator.Generate();
        }
    }
}
