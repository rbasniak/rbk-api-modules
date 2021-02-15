using rbkApiModules.Utilities.Testing;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace rbkApiModules.Infrastructure.Models.Tests
{
    public class ValidationTests
    {
        [AutoNamedFact]
        public void Should_Have_Required_When_Required_And_Empty()
        {
            var instance = new TestRequired { RequiredTest = "" };

            Should.Throw<ModelValidationException>(
                () => instance.Validate()).Errors.Any(x => x.PropertyName == nameof(TestRequired.RequiredTest) && x.Results.Any(x => x.Type == ValidationType.Required)
            );
        }

        [AutoNamedFact]
        public void Should_Not_Have_Required_When_Required_And_Not_Empty()
        {
            var instance = new TestRequired { RequiredTest = "aa" };

            Should.NotThrow(() => instance.Validate());
        }

        [AutoNamedFact]
        public void Should_Not_Have_MinLen_When_NotRequired_And_Empty()
        {
            var instance = new TestNotRequiredMinLength { NotRequiredMinLengthTest = "" };

            Should.NotThrow(() => instance.Validate());
        }

        [AutoNamedFact]
        public void Should_Have_MinLen_When_NotRequired_And_Not_Empty_And_Wrong_Size()
        {
            var instance = new TestNotRequiredMinLength { NotRequiredMinLengthTest = "aa" };

            Should.Throw<ModelValidationException>(
                () => instance.Validate()).Errors.Any(x => x.PropertyName == nameof(TestNotRequiredMinLength.NotRequiredMinLengthTest) && x.Results.Any(x => x.Type == ValidationType.MinLength)
            );
        }

        [AutoNamedFact]
        public void Should_Have_MinLen_When_Required_And_Empty()
        {
            var instance = new TestRequiredMinLength { RequiredMinLengthTest = "" };

            Should.Throw<ModelValidationException>(
                () => instance.Validate()).Errors.Any(x => x.PropertyName == nameof(TestRequiredMinLength.RequiredMinLengthTest) && x.Results.Any(x => x.Type == ValidationType.MinLength)
            );
        }

        [AutoNamedFact]
        public void Should_Have_MinLen_When_Required_And_NotEmpty_And_Wrong_Size()
        {
            var instance = new TestRequiredMinLength { RequiredMinLengthTest = "aa" };

            Should.Throw<ModelValidationException>(
                () => instance.Validate()).Errors.Any(x => x.PropertyName == nameof(TestRequiredMinLength.RequiredMinLengthTest) && x.Results.Any(x => x.Type == ValidationType.MinLength)
            );
        }

        [AutoNamedFact]
        public void Should_Not_Have_MinLen_When_Required_And_NotEmpty_And_Right_Size()
        {
            var instance = new TestRequiredMinLength { RequiredMinLengthTest = "aaa" };

            Should.NotThrow(() => instance.Validate());
        }

        [AutoNamedFact]
        public void Should_Not_Have_MaxLen_When_NotRequired_And_Empty()
        {
            var instance = new TestNotRequiredMaxLength { NotRequiredMaxLengthTest = "" };

            Should.NotThrow(() => instance.Validate());
        }

        [AutoNamedFact]
        public void Should_Have_MaxLen_When_NotRequired_And_Not_Empty_And_Wrong_Size()
        {
            var instance = new TestNotRequiredMaxLength { NotRequiredMaxLengthTest = "aaaa" };

            Should.Throw<ModelValidationException>(
                () => instance.Validate()).Errors.Any(x => x.PropertyName == nameof(TestNotRequiredMaxLength.NotRequiredMaxLengthTest) && x.Results.Any(x => x.Type == ValidationType.MaxLength)
            );
        }

        [AutoNamedFact]
        public void Should_Have_MaxLen_When_Required_And_Empty()
        {
            var instance = new TestRequiredMaxLength { RequiredMaxLengthTest = "" };

            Should.Throw<ModelValidationException>(
                () => instance.Validate()).Errors.Any(x => x.PropertyName == nameof(TestRequiredMaxLength.RequiredMaxLengthTest) && x.Results.Any(x => x.Type == ValidationType.MaxLength)
            );
        }

        [AutoNamedFact]
        public void Should_Have_MaxLen_When_Required_And_NotEmpty_And_Wrong_Size()
        {
            var instance = new TestRequiredMaxLength { RequiredMaxLengthTest = "aaaa" };

            Should.Throw<ModelValidationException>(
                () => instance.Validate()).Errors.Any(x => x.PropertyName == nameof(TestRequiredMaxLength.RequiredMaxLengthTest) && x.Results.Any(x => x.Type == ValidationType.MaxLength)
            );
        }

        [AutoNamedFact]
        public void Should_Not_Have_MaxLen_When_Required_And_NotEmpty_And_Right_Size()
        {
            var instance = new TestRequiredMaxLength { RequiredMaxLengthTest = "aaa" };

            Should.NotThrow(() => instance.Validate());
        }
    }

    public class TestRequired
    {
        [Required]
        public string RequiredTest { get; set; }
    }

    public class TestNotRequiredMinLength
    {
        [MinLength(3)]
        public string NotRequiredMinLengthTest { get; set; }
    }

    public class TestRequiredMinLength
    {
        [Required, MinLength(3)]
        public string RequiredMinLengthTest { get; set; }
    }

    public class TestNotRequiredMaxLength
    {
        [MaxLength(3)]
        public string NotRequiredMaxLengthTest { get; set; }
    }

    public class TestRequiredMaxLength
    {
        [Required, MaxLength(3)]
        public string RequiredMaxLengthTest { get; set; }
    }
}
