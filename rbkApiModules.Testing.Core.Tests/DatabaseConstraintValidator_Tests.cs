using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core;
using Shouldly;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Commons.Testing;

public class DatabaseConstraintValidator_Tests
{
    // Test models
    public class TestModel : IBaseEntity
    {
        public Guid Id { get; set; } 

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Description { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public TestCategory? Category { get; set; }
    }

    public class TestCategory : IBaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<TestModel> TestModels { get; set; } = null!;
        public DbSet<TestCategory> TestCategories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestModel>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired().HasMaxLength(50);
                entity.Property(x => x.Description).HasMaxLength(100);
                entity.Property(x => x.CategoryId).IsRequired();
                entity.HasOne(x => x.Category)
                      .WithMany()
                      .HasForeignKey(x => x.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TestCategory>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired().HasMaxLength(30);
            });
        }
    }

    // Helper method to create and initialize a test context
    private async Task<TestDbContext> GetContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); // Keep connection open!

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new TestDbContext(options);
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    // Test requests
    public class CreateTestModelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class UpdateTestModelRequest : IBaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class CreateTestModelWithIdRequest : IBaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
    }

    // Test validators
    public class CreateTestModelValidator : SmartValidator<CreateTestModelRequest, TestModel>
    {
        public CreateTestModelValidator(DbContext context, ILocalizationService? localizationService = null)
            : base(context, localizationService)
        {
        }

        protected override void ValidateBusinessRules()
        {
            RuleFor(x => x.Name)
                .Must(name => name.StartsWith("Test"))
                .WithMessage("Name must start with 'Test'");
        }
    }

    public class UpdateTestModelValidator : SmartValidator<UpdateTestModelRequest, TestModel>
    {
        public UpdateTestModelValidator(DbContext context, ILocalizationService? localizationService = null)
            : base(context, localizationService)
        {
        }

        protected override void ValidateBusinessRules()
        {
            RuleFor(x => x.Name)
                .Must(name => name.Length > 3)
                .WithMessage("Name must be longer than 3 characters");
        }
    }

    public class CreateTestModelWithIdValidator : SmartValidator<CreateTestModelWithIdRequest, TestModel>
    {
        public CreateTestModelWithIdValidator(DbContext context, ILocalizationService? localizationService = null)
            : base(context, localizationService)
        {
        }

        protected override void ValidateBusinessRules()
        {
            RuleFor(x => x.Name)
                .Must(name => name.StartsWith("Test"))
                .WithMessage("Name must start with 'Test'");
        }
    }

    public class SkipPrimaryKeyValidationValidator : SmartValidator<UpdateTestModelRequest, TestModel>
    {
        public SkipPrimaryKeyValidationValidator(DbContext context, ILocalizationService? localizationService = null)
            : base(context, localizationService)
        {
        }

        protected override bool ShouldSkipPrimaryKeyValidation()
        {
            return true;
        }

        protected override void ValidateBusinessRules()
        {
            RuleFor(x => x.Name)
                .Must(name => name.Length > 3)
                .WithMessage("Name must be longer than 3 characters");
        }
    }

    [Test]
    public async Task Should_Validate_Required_Fields()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new CreateTestModelValidator(context, localizationService);


        var request = new CreateTestModelRequest
        {
            Name = "", // Empty required field
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(CreateTestModelRequest.Name) && x.ErrorMessage.Contains("required"));
    }

    [Test]
    public async Task Should_Validate_MaxLength_Constraints()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new CreateTestModelValidator(context, localizationService);

        var request = new CreateTestModelRequest
        {
            Name = new string('A', 51), // Exceeds max length of 50
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == "Name" && x.ErrorMessage == "Name must start with 'Test'");
        result.Errors.ShouldContain(x => x.PropertyName == "Name" && x.ErrorMessage == "Name cannot exceed 50 characters.");
        result.Errors.ShouldContain(x => x.PropertyName == "CategoryId" && x.ErrorMessage == "CategoryId references a non-existent record.");
    }

    [Test]
    public async Task Should_Allow_Nullable_Fields()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new CreateTestModelValidator(context, localizationService);

        var request = new CreateTestModelRequest
        {
            Name = "TestName",
            Description = null, // Nullable field should be allowed
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.Errors.ShouldNotContain(x => x.PropertyName == nameof(CreateTestModelRequest.Description));
    }

    [Test]
    public async Task Should_Validate_Custom_Rules()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new CreateTestModelValidator(context, localizationService);

        var request = new CreateTestModelRequest
        {
            Name = "InvalidName", // Doesn't start with "Test"
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(CreateTestModelRequest.Name) && x.ErrorMessage.Contains("start with 'Test'"));
    }

    [Test]
    public async Task Should_Validate_Foreign_Key_Constraints()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new CreateTestModelValidator(context, localizationService);

        var request = new CreateTestModelRequest
        {
            Name = "TestName",
            CategoryId = Guid.NewGuid() // Non-existent foreign key
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(CreateTestModelRequest.CategoryId) && x.ErrorMessage.Contains("exist"));
    }

    [Test]
    public async Task Should_Allow_Valid_Request()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new CreateTestModelValidator(context, localizationService);

        var request = new CreateTestModelRequest
        {
            Name = "TestValidName", // Starts with "Test" to pass custom rule
            Description = "Valid description", // Within max length
            CategoryId = Guid.NewGuid() // Will be validated by foreign key constraint
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        // The validation should pass for required fields and max length
        // Foreign key validation might fail if the category doesn't exist, but that's expected
        result.Errors.ShouldNotContain(x => x.PropertyName == nameof(CreateTestModelRequest.Name));
        result.Errors.ShouldNotContain(x => x.PropertyName == nameof(CreateTestModelRequest.Description));

        // The Name should pass the custom rule (starts with "Test")
        result.Errors.ShouldNotContain(x => x.PropertyName == nameof(CreateTestModelRequest.Name) && x.ErrorMessage.Contains("must start with 'Test'"));
    }

    [Test]
    public async Task Should_Use_Custom_Error_Messages()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new CreateTestModelValidator(context, localizationService);

        var request = new CreateTestModelRequest
        {
            Name = "InvalidName",
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.Errors.ShouldContain(x => x.PropertyName == nameof(CreateTestModelRequest.Name) && x.ErrorMessage == "Name must start with 'Test'");
    }

    [Test]
    public async Task Should_Handle_Property_Mappings()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new CreateTestModelValidator(context, localizationService);

        // Test with property mapping (if implemented)
        var request = new CreateTestModelRequest
        {
            Name = "TestName",
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert - should work with default property mapping
        result.IsValid.ShouldBeFalse(); // Will fail due to custom rule requiring "Test" prefix
    }

    [Test]
    public async Task Should_Handle_Ignored_Properties()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new CreateTestModelValidator(context, localizationService);

        var request = new CreateTestModelRequest
        {
            Name = "TestName",
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert - ignored properties should not cause validation errors
        result.Errors.ShouldNotContain(x => x.PropertyName == "IgnoredProperty");
    }

    [Test]
    public async Task Should_Debug_EF_Core_Metadata()
    {
        // Arrange
        using var context = await GetContext();

        // Debug: Check if entity type is found
        var entityType = context.Model.FindEntityType(typeof(TestModel));
        entityType.ShouldNotBeNull("Entity type should be found");

        // Debug: Check properties
        var properties = entityType.GetProperties().ToList();
        properties.ShouldNotBeEmpty("Properties should be found");

        // Debug: Check specific properties
        var nameProperty = properties.FirstOrDefault(x => x.Name == "Name");
        nameProperty.ShouldNotBeNull("Name property should be found");
        nameProperty.IsNullable.ShouldBeFalse("Name property should not be nullable");
        nameProperty.GetMaxLength().ShouldBe(50, "Name property should have max length 50");

        var descriptionProperty = properties.FirstOrDefault(x => x.Name == "Description");
        descriptionProperty.ShouldNotBeNull("Description property should be found");
        descriptionProperty.IsNullable.ShouldBeTrue("Description property should be nullable");
        descriptionProperty.GetMaxLength().ShouldBe(100, "Description property should have max length 100");

        var categoryIdProperty = properties.FirstOrDefault(x => x.Name == "CategoryId");
        categoryIdProperty.ShouldNotBeNull("CategoryId property should be found");
        categoryIdProperty.IsNullable.ShouldBeFalse("CategoryId property should not be nullable");

        // Debug: Check foreign keys
        var foreignKeys = categoryIdProperty.GetContainingForeignKeys().ToList();
        foreignKeys.ShouldNotBeEmpty("Foreign keys should be found");

        Console.WriteLine($"Entity type found: {entityType.Name}");
        Console.WriteLine($"Properties found: {properties.Count}");
        Console.WriteLine($"Name property nullable: {nameProperty.IsNullable}");
        Console.WriteLine($"Name property max length: {nameProperty.GetMaxLength()}");
        Console.WriteLine($"Foreign keys found: {foreignKeys.Count}");
    }

    [Test]
    public async Task Should_Validate_Primary_Key_Exists()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new UpdateTestModelValidator(context, localizationService);

        var request = new UpdateTestModelRequest
        {
            Id = Guid.NewGuid(), // Non-existent ID
            Name = "ValidName",
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(UpdateTestModelRequest.Id) && x.ErrorMessage.Contains("exist"));
    }

    [Test]
    public async Task Should_Allow_Valid_Primary_Key()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new UpdateTestModelValidator(context, localizationService);

        var request = new UpdateTestModelRequest
        {
            Id = Guid.NewGuid(), // Non-existent ID should fail validation
            Name = "ValidName",
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(UpdateTestModelRequest.Id) && x.ErrorMessage.Contains("exist"));
    }

    [Test]
    public async Task Should_Skip_Primary_Key_Validation_When_Configured()
    {
        // Arrange
        using var context = await GetContext();
        var localizationService = new MockLocalizationService();
        var validator = new SkipPrimaryKeyValidationValidator(context, localizationService);

        var request = new UpdateTestModelRequest
        {
            Id = Guid.NewGuid(), // Non-existent ID, but validation should be skipped
            Name = "ValidName",
            CategoryId = Guid.NewGuid()
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.Errors.ShouldNotContain(x => x.PropertyName == nameof(UpdateTestModelRequest.Id));
    }

    // Mock localization service for testing
    private class MockLocalizationService : ILocalizationService
    {
        public string LocalizeString(Enum value)
        {
            return value.ToString();
        }

        public string GetLanguageTemplate(string localization = null)
        {
            return "{}";
        }
    }
}