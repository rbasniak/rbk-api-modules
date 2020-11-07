using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace rbkApiModules.Infrastructure.Api.Tests
{
    public class ApplicationAreaAttributeTests
    {
        [Fact]
        public void Is_Attribute_Multiple_False()
        {
            var attributes = (IList<AttributeUsageAttribute>)typeof(ApplicationAreaAttribute).GetCustomAttributes(typeof(AttributeUsageAttribute), false);

            attributes.Count.ShouldBe(1);
            attributes.First().AllowMultiple.ShouldBe(false);
            attributes.First().Inherited.ShouldBe(false);
            attributes.First().AllowMultiple.ShouldBe(false);
        }

        [Fact]
        public void Is_Constructor_Initializing_Properties()
        {
            var area = "My Custom Area";
            var attribute = new ApplicationAreaAttribute(area);

            attribute.Area.ShouldBe(area);
        }
    }
}
