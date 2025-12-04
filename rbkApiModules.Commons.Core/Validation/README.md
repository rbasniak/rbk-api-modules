# Database Constraint Validator

This system automatically syncs Entity Framework database constraints with FluentValidation rules, ensuring that validation rules stay in sync with database schema changes.

## Overview

The `DatabaseConstraintValidator<TRequest, TModel>` base class automatically applies the following validation rules based on your Entity Framework configuration:

- **Required fields**: Automatically adds `NotEmpty()` or `NotNull()` validation for non-nullable properties
- **Max length**: Automatically adds `MaximumLength()` validation for string properties with `HasMaxLength()` configured
- **Foreign keys**: Automatically validates that foreign key values exist in the referenced table

## Basic Usage

```csharp
public class CreateMaterialValidator : DatabaseConstraintValidator<CreateMaterialRequest, Material>
{
    public CreateMaterialValidator(DbContext context, ILocalizationService? localization = null) 
        : base(context, localization)
    {
    }

    protected override void ApplyCustomRules()
    {
        // Add your custom business rules here
        RuleFor(x => x.Name)
            .MustAsync(async (name, cancellationToken) =>
                !await Context.Set<Material>().AnyAsync(x => x.Name == name, cancellationToken))
            .WithMessage("A material with this name already exists.");
    }
}
```

## Database Configuration Example

```csharp
public class MaterialConfig : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.Unit)
            .IsRequired();
        
        builder.Property(x => x.PricePerUnit)
            .IsRequired();
    }
}
```

## Advanced Usage

### Property Mappings

Use property mappings when your request properties have different names than your model properties:

```csharp
protected override Dictionary<string, string> GetPropertyMappings()
{
    return new Dictionary<string, string>
    {
        { "LineId", "PaintLineId" },  // Model property -> Request property
        { "CategoryId", "MaterialCategoryId" }
    };
}
```

### Ignoring Properties

Exclude certain properties from automatic validation:

```csharp
protected override IEnumerable<string> GetIgnoredProperties()
{
    return new[] { "Id", "CreatedAt", "UpdatedAt" };
}
```

### Custom Rules

Add business-specific validation rules:

```csharp
protected override void ApplyCustomRules()
{
    // Custom format validation
    RuleFor(x => x.HexColor)
        .Matches("^#[0-9A-Fa-f]{6}$")
        .WithMessage("Hex color must be in format #RRGGBB");

    // Business logic validation
    RuleFor(x => x.Price)
        .GreaterThan(0)
        .WithMessage("Price must be greater than zero.");

    // Complex validation
    RuleFor(x => x.Name)
        .MustAsync(async (name, cancellationToken) =>
            !await Context.Set<Material>().AnyAsync(x => x.Name == name, cancellationToken))
        .WithMessage("Name must be unique.");
}
```

## Supported Constraint Types

### Required Fields
- String properties: `NotEmpty()`
- Guid properties: `NotEmpty()`
- Other reference types: `NotNull()`
- Value types: No validation (can't be null)

### Max Length
- String properties with `HasMaxLength()`: `MaximumLength()`

### Foreign Keys
- Properties with foreign key relationships: Validates existence in referenced table

## Error Messages

The system provides default error messages, but you can customize them using localization:

```csharp
// In your localization service
"Validation.Required" = "{PropertyName} is required."
"Validation.MaxLength" = "{PropertyName} cannot exceed {Parameter} characters."
"Validation.ForeignKeyNotFound" = "{PropertyName} references a non-existent record."
```

## Benefits

1. **Automatic Sync**: Database constraints and validation rules stay in sync automatically
2. **DRY Principle**: No need to duplicate constraint definitions
3. **Type Safety**: Compile-time checking of property mappings
4. **Flexibility**: Easy to add custom business rules
5. **Maintainability**: Changes to database schema automatically update validation

## Migration Guide

To migrate existing validators:

1. Change the base class from `AbstractValidator<T>` to `DatabaseConstraintValidator<TRequest, TModel>`
2. Move constructor logic to `ApplyCustomRules()`
3. Remove duplicate validation rules that are now handled automatically
4. Add property mappings if needed
5. Specify ignored properties if needed

### Before
```csharp
public class Validator : AbstractValidator<Request>
{
    public Validator(DbContext context)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Unit).NotEmpty();
        RuleFor(x => x.PricePerUnit).GreaterThan(0);
    }
}
```

### After
```csharp
public class Validator : DatabaseConstraintValidator<Request, Material>
{
    public Validator(DbContext context) : base(context)
    {
    }

    protected override void ApplyCustomRules()
    {
        RuleFor(x => x.PricePerUnit).GreaterThan(0);
    }
}
```

## Best Practices

1. **Use for all CRUD operations**: Apply to Create, Update, and other commands that modify data
2. **Keep custom rules focused**: Only add business-specific validation in `ApplyCustomRules()`
3. **Use property mappings sparingly**: Try to keep request and model property names consistent
4. **Test thoroughly**: Ensure custom rules work correctly with automatic constraints
5. **Document exceptions**: When you need to ignore properties, document why

## Troubleshooting

### Entity Type Not Found
Make sure your model is registered in the DbContext and the generic type parameter is correct.

### Property Not Found
Check that the property exists in both the request and model classes, or use property mappings.

### Foreign Key Validation Issues
Ensure the foreign key relationship is properly configured in your Entity Framework configuration. 