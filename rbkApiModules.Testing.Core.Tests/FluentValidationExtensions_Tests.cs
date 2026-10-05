using FluentValidation;
using rbkApiModules.Commons.Core;
using Shouldly;

namespace rbkApiModules.Commons.Testing;

public class FluentValidationExtensions_Tests
{
    private enum SampleStatus
    {
        Active = 1,
        Inactive = 2
    }

    private sealed class MockLocalizationService : ILocalizationService
    {
        public string LocalizeString(Enum value) => value.ToString();

        public string GetLanguageTemplate(string localization = null) => "{}";
    }

    private sealed class CollectionRequest
    {
        public string[]? Tags { get; set; }
        public List<string>? Items { get; set; }
    }

    private sealed class TagsIsRequiredValidator : AbstractValidator<CollectionRequest>
    {
        public TagsIsRequiredValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Tags).IsRequired(localization);
        }
    }

    private sealed class TagsMustHaveItemsValidator : AbstractValidator<CollectionRequest>
    {
        public TagsMustHaveItemsValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Tags).IsRequired(localization).MustHaveItems(localization);
        }
    }

    private sealed class ListMustHaveItemsValidator : AbstractValidator<CollectionRequest>
    {
        public ListMustHaveItemsValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Items).MustHaveItems(localization);
        }
    }

    private sealed class NullableValueRequest
    {
        public int? Quantity { get; set; }
        public decimal? Amount { get; set; }
    }

    private sealed class NullableValueValidator : AbstractValidator<NullableValueRequest>
    {
        public NullableValueValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Quantity).IsRequired(localization);
            RuleFor(x => x.Amount).IsRequired(localization);
        }
    }

    private sealed class EnumRequest
    {
        public SampleStatus? Status { get; set; }
    }

    private sealed class EnumValidator : AbstractValidator<EnumRequest>
    {
        public EnumValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Status).IsRequired(localization);
            RuleFor(x => x.Status).MustBeInEnum(localization);
        }
    }

    private sealed class EnumOnlyValidator : AbstractValidator<EnumRequest>
    {
        public EnumOnlyValidator(ILocalizationService localization)
        {
            RuleFor(x => x.Status).MustBeInEnum(localization);
        }
    }

    private readonly MockLocalizationService _localization = new();

    [Test]
    public async Task IsRequired_On_Collection_Should_Fail_When_Null()
    {
        var validator = new TagsIsRequiredValidator(_localization);
        var result = await validator.ValidateAsync(new CollectionRequest { Tags = null });

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x =>
            x.PropertyName == nameof(CollectionRequest.Tags)
            && x.ErrorMessage == SharedValidationMessages.Common.FieldCannotBeNull.ToString());
    }

    [Test]
    public async Task IsRequired_On_Collection_Should_Pass_When_Empty()
    {
        var validator = new TagsIsRequiredValidator(_localization);
        var result = await validator.ValidateAsync(new CollectionRequest { Tags = [] });

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task IsRequired_On_Collection_Should_Pass_When_Has_Item()
    {
        var validator = new TagsIsRequiredValidator(_localization);
        var result = await validator.ValidateAsync(new CollectionRequest { Tags = ["a"] });

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task MustHaveItems_Should_Fail_When_Null_Or_Empty()
    {
        var validator = new TagsMustHaveItemsValidator(_localization);

        (await validator.ValidateAsync(new CollectionRequest { Tags = null })).IsValid.ShouldBeFalse();
        (await validator.ValidateAsync(new CollectionRequest { Tags = [] })).IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task MustHaveItems_Should_Pass_When_Has_Item()
    {
        var validator = new TagsMustHaveItemsValidator(_localization);
        var result = await validator.ValidateAsync(new CollectionRequest { Tags = ["x"], Items = ["y"] });

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task MustHaveItems_On_List_Should_Fail_When_Empty()
    {
        var validator = new ListMustHaveItemsValidator(_localization);
        var result = await validator.ValidateAsync(new CollectionRequest { Tags = ["ok"], Items = [] });

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(CollectionRequest.Items));
    }

    [Test]
    public async Task IsRequired_On_Nullable_Int_Should_Fail_When_Null()
    {
        var validator = new NullableValueValidator(_localization);
        var result = await validator.ValidateAsync(new NullableValueRequest { Quantity = null, Amount = 1m });

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(NullableValueRequest.Quantity));
    }

    [Test]
    public async Task IsRequired_On_Nullable_Int_Should_Pass_When_Zero()
    {
        var validator = new NullableValueValidator(_localization);
        var result = await validator.ValidateAsync(new NullableValueRequest { Quantity = 0, Amount = 0m });

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task IsRequired_On_Nullable_Decimal_Should_Fail_When_Null()
    {
        var validator = new NullableValueValidator(_localization);
        var result = await validator.ValidateAsync(new NullableValueRequest { Quantity = 1, Amount = null });

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(NullableValueRequest.Amount));
    }

    [Test]
    public async Task MustBeInEnum_Should_Pass_When_Null_On_Nullable_Property()
    {
        var validator = new EnumOnlyValidator(_localization);
        var result = await validator.ValidateAsync(new EnumRequest { Status = null });
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task MustBeInEnum_Should_Pass_When_Defined()
    {
        var validator = new EnumValidator(_localization);
        var result = await validator.ValidateAsync(new EnumRequest { Status = SampleStatus.Active });

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task MustBeInEnum_Should_Fail_When_Undefined_Value()
    {
        var validator = new EnumValidator(_localization);
        var result = await validator.ValidateAsync(new EnumRequest { Status = (SampleStatus)99 });

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x =>
            x.PropertyName == nameof(EnumRequest.Status)
            && x.ErrorMessage == SharedValidationMessages.Common.FieldHasInvalidValue.ToString());
    }

    [Test]
    public async Task IsRequired_On_Nullable_Enum_Should_Fail_When_Null()
    {
        var validator = new EnumValidator(_localization);
        var result = await validator.ValidateAsync(new EnumRequest { Status = null });

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x =>
            x.PropertyName == nameof(EnumRequest.Status)
            && x.ErrorMessage == SharedValidationMessages.Common.FieldCannotBeNull.ToString());
    }
}
