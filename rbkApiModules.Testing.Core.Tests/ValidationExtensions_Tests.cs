using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Validation;
using Shouldly;

namespace rbkApiModules.Commons.Testing;

public class ValidationExtensions_Tests
{
    private sealed class TestTenantEntity : TenantEntity
    {
        public TestTenantEntity(Guid id, string? tenantId)
        {
            Id = id;
            TenantId = tenantId;
        }
    }

    private sealed class TestTenantDbContext : DbContext
    {
        public TestTenantDbContext(DbContextOptions<TestTenantDbContext> options) : base(options) { }

        public DbSet<TestTenantEntity> TestTenantEntities { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestTenantEntity>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.TenantId).HasMaxLength(255);
            });
        }
    }

    private sealed class TestRequest : AuthenticatedRequest
    {
        public Guid EntityId { get; set; }
        public Guid? OptionalEntityId { get; set; }
    }

    private sealed class TestValidator : AbstractValidator<TestRequest>
    {
        public TestValidator(DbContext context, ILocalizationService localization)
        {
            RuleFor(x => x.EntityId)
                .MustExistInDatabaseForCurrentTenant<TestRequest, TestTenantEntity>(context, localization);

            RuleFor(x => x.OptionalEntityId)
                .MustExistInDatabaseForCurrentTenantWhenNotNull<TestRequest, TestTenantEntity>(context, localization);
        }
    }

    private sealed class MockLocalizationService : ILocalizationService
    {
        public string LocalizeString(Enum value) => value.ToString();

        public string GetLanguageTemplate(string localization = null) => "{}";
    }

    private static readonly Guid TenantAEntityId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid TenantBEntityId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid GlobalEntityId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid MissingEntityId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    private async Task<TestTenantDbContext> CreateSeededContextAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestTenantDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new TestTenantDbContext(options);
        await context.Database.EnsureCreatedAsync();

        context.TestTenantEntities.AddRange(
            new TestTenantEntity(TenantAEntityId, "TENANT-A"),
            new TestTenantEntity(TenantBEntityId, "TENANT-B"),
            new TestTenantEntity(GlobalEntityId, null));

        await context.SaveChangesAsync();
        return context;
    }

    private static TestRequest CreateRequest(string? tenant, Guid entityId, Guid? optionalEntityId = null)
    {
        var request = new TestRequest
        {
            EntityId = entityId,
            OptionalEntityId = optionalEntityId
        };

        request.SetIdentity(tenant, "test-user", []);
        return request;
    }

    [Test]
    public async Task MustExistInDatabaseForCurrentTenant_Should_Pass_When_Entity_Belongs_To_User_Tenant()
    {
        await using var context = await CreateSeededContextAsync();
        var validator = new TestValidator(context, new MockLocalizationService());
        var request = CreateRequest("TENANT-A", TenantAEntityId);

        var result = await validator.ValidateAsync(request);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task MustExistInDatabaseForCurrentTenant_Should_Fail_When_Entity_Belongs_To_Different_Tenant()
    {
        await using var context = await CreateSeededContextAsync();
        var validator = new TestValidator(context, new MockLocalizationService());
        var request = CreateRequest("TENANT-A", TenantBEntityId);

        var result = await validator.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(TestRequest.EntityId));
    }

    [Test]
    public async Task MustExistInDatabaseForCurrentTenant_Should_Fail_When_Entity_Does_Not_Exist()
    {
        await using var context = await CreateSeededContextAsync();
        var validator = new TestValidator(context, new MockLocalizationService());
        var request = CreateRequest("TENANT-A", MissingEntityId);

        var result = await validator.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(TestRequest.EntityId));
    }

    [Test]
    public async Task MustExistInDatabaseForCurrentTenant_Should_Pass_For_Global_Admin_When_Entity_Is_Global()
    {
        await using var context = await CreateSeededContextAsync();
        var validator = new TestValidator(context, new MockLocalizationService());
        var request = CreateRequest(String.Empty, GlobalEntityId);

        var result = await validator.ValidateAsync(request);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task MustExistInDatabaseForCurrentTenant_Should_Fail_When_Tenant_Is_Null()
    {
        await using var context = await CreateSeededContextAsync();
        var validator = new TestValidator(context, new MockLocalizationService());
        var request = CreateRequest(null, TenantAEntityId);

        var result = await validator.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(TestRequest.EntityId));
    }

    [Test]
    public async Task MustExistInDatabaseForCurrentTenant_Should_Fail_For_Global_Admin_When_Entity_Has_Tenant()
    {
        await using var context = await CreateSeededContextAsync();
        var validator = new TestValidator(context, new MockLocalizationService());
        var request = CreateRequest(String.Empty, TenantAEntityId);

        var result = await validator.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(TestRequest.EntityId));
    }

    [Test]
    public async Task MustExistInDatabaseForCurrentTenant_Should_Fail_For_Tenant_User_When_Entity_Is_Global()
    {
        await using var context = await CreateSeededContextAsync();
        var validator = new TestValidator(context, new MockLocalizationService());
        var request = CreateRequest("TENANT-A", GlobalEntityId);

        var result = await validator.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(TestRequest.EntityId));
    }

    [Test]
    public async Task MustExistInDatabaseForCurrentTenantWhenNotNull_Should_Pass_When_Optional_Id_Is_Null()
    {
        await using var context = await CreateSeededContextAsync();
        var validator = new TestValidator(context, new MockLocalizationService());
        var request = CreateRequest("TENANT-A", TenantAEntityId, optionalEntityId: null);

        var result = await validator.ValidateAsync(request);

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task MustExistInDatabaseForCurrentTenantWhenNotNull_Should_Validate_When_Optional_Id_Is_Provided()
    {
        await using var context = await CreateSeededContextAsync();
        var validator = new TestValidator(context, new MockLocalizationService());
        var request = CreateRequest("TENANT-A", TenantAEntityId, optionalEntityId: TenantBEntityId);

        var result = await validator.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(x => x.PropertyName == nameof(TestRequest.OptionalEntityId));
    }
}
