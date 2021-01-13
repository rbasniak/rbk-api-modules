using rbkApiModules.Infrastructure.Utilities.EFCore.Converters;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace rbkApiModules.Infrastructure.Utilities.EFCore.Converters.Tests
{
    public class ArrayOfStringsConverterTests
    {
        [Fact]
        public void Should_Convert_Null_Array()
        {
            var converter = ArrayOfStringsConverter.GetConverter(';');
            string[] value = null;
            var result = converter.ConvertToProvider(value);

            result.ShouldBe(null);
        }

        [Fact]
        public void Should_Convert_Zero_Length_Array()
        {
            var converter = ArrayOfStringsConverter.GetConverter(';');
            var result = converter.ConvertToProvider(new string[0]);

            result.ToString().ShouldBe(String.Empty);
        }

        [Fact]
        public void Should_Convert_One_Length_Array()
        {
            var converter = ArrayOfStringsConverter.GetConverter(';');
            var result = converter.ConvertToProvider(new string[] { "v1" });

            result.ToString().ShouldBe("v1");
        }

        [Fact]
        public void Should_Convert_And_Order_Any_Array()
        {
            var converter = ArrayOfStringsConverter.GetConverter(';');
            var result = converter.ConvertToProvider(new string[] { "v1", "v3", "v2" });

            result.ToString().ShouldBe("v1;v2;v3");
        }

        [Fact]
        public void Should_Convert_Null_String()
        {
            var converter = ArrayOfStringsConverter.GetConverter(';');
            var result = converter.ConvertFromProvider(null); 

            result.ShouldBeNull();
        }

        [Fact]
        public void Should_Convert_Empty_String()
        {
            var converter = ArrayOfStringsConverter.GetConverter(';');
            var result = converter.ConvertFromProvider(String.Empty);

            result.ShouldNotBeNull();
            (result as string[]).Length.ShouldBe(0);
        }

        [Fact]
        public void Should_Convert_Single_String()
        {
            var converter = ArrayOfStringsConverter.GetConverter(';');
            var result = converter.ConvertFromProvider("v1");

            result.ShouldNotBeNull();
            (result as string[]).Length.ShouldBe(1);
            (result as string[])[0].ShouldBe("v1");
        }

        [Fact]
        public void Should_Convert_Many_Strings()
        {
            var converter = ArrayOfStringsConverter.GetConverter(';');
            var result = converter.ConvertFromProvider("v1;v2;v3");

            result.ShouldNotBeNull();
            (result as string[]).Length.ShouldBe(3);
            (result as string[])[0].ShouldBe("v1");
            (result as string[])[1].ShouldBe("v2");
            (result as string[])[2].ShouldBe("v3");
        }
    }
}
