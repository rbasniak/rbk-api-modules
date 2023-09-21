using Azure.Core;
using Demo1.BusinessLogic.Commands;
using Demo1.DataTransfer;
using Demo1.Models.Domain.Demo;
using rbkApiModules.Faqs.Core;

namespace rbkApiModules.Demo1.Tests.Integration.Faqs;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class IDomainEntityValidator_Tests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    private static Guid _author;
    private static Guid _blogFromTenant1;
    private static Guid _blogFromTenant2;
    private static string _userFromTenant1;
    private static string _userFromTenant2;

    public IDomainEntityValidator_Tests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Seed()
    {
        var context = _serverFixture.Context;

        var tenant1 = context.Add(new Tenant("TENANT1", "Tenant 1")).Entity;
        var tenant2 = context.Add(new Tenant("TENANT2", "Tenant 2")).Entity;

        var author = context.Set<Author>().Add(new Author("John Doe")).Entity;
        var blogFromTenant1 = context.Set<Blog>().Add(new Blog(tenant1.Alias, "John Doe's Life")).Entity;
        var blogFromTenant2 = context.Set<Blog>().Add(new Blog(tenant2.Alias, "John Doe's Life")).Entity;

        var user1 = context.Set<User>().Add(new User(tenant1.Alias, "user1", "user1@tenant1.com", "123", "", "User 1", AuthenticationMode.Credentials)).Entity;
        var user2 = context.Set<User>().Add(new User(tenant2.Alias, "user2", "user2@tenant1.com", "123", "", "User 2", AuthenticationMode.Credentials)).Entity;

        user1.Confirm();
        user2.Confirm();

        context.SaveChanges();

        _author = author.Id;
        _blogFromTenant1 = blogFromTenant1.Id;
        _blogFromTenant2 = blogFromTenant2.Id;
        _userFromTenant1 = await _serverFixture.GetAccessTokenAsync("user1", "123", tenant1.Alias);
        _userFromTenant2 = await _serverFixture.GetAccessTokenAsync("user2", "123", tenant2.Alias);
    }

    [FriendlyNamedFact("IT-P010"), Priority(10)]
    public async Task Domain_database_required_restraint_should_be_validate_when_IDomainEntityValidator_is_implemented()
    {
        // Prepare
        var request = new CreatePost.Request
        {
            Title = "",
            Body = "asdfgasdfgasdfgasdfgasdfgasdfgasdfgasdfg",
            AuthorId = _author,
            BlogId = _blogFromTenant1,
        };

        // Act
        var response = await _serverFixture.PostAsync("api/blogs/posts", request, credentials: _userFromTenant1);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Title' is required");
    }

    [FriendlyNamedFact("IT-P020"), Priority(20)]
    public async Task Domain_database_minlength_restraint_should_be_validate_when_IDomainEntityValidator_is_implemented()
    {
        // Prepare
        var request = new CreatePost.Request
        {
            Title = "a",
            Body = "a",
            AuthorId = _author,
            BlogId = _blogFromTenant1,
        };

        // Act
        var response = await _serverFixture.PostAsync("api/blogs/posts", request, credentials: _userFromTenant1);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Title' must have between 2 and 16 characters", "The field 'Body' must have between 32 and 4096 characters");
    }


    [FriendlyNamedFact("IT-P030"), Priority(30)]
    public async Task Domain_database_maxlength_restraint_should_be_validate_when_IDomainEntityValidator_is_implemented()
    {
        // Prepare
        var request = new CreatePost.Request
        {
            Title = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            Body = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            AuthorId = null,
            BlogId = _blogFromTenant1,
        };

        // Act
        var response = await _serverFixture.PostAsync("api/blogs/posts", request, credentials: _userFromTenant1);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Title' must have between 2 and 16 characters");
    }

    [FriendlyNamedFact("IT-P040"), Priority(40)]
    public async Task Domain_database_exists_in_database_restraint_should_be_validate_when_IDomainEntityValidator_is_implemented()
    {
        // Prepare
        var request = new CreatePost.Request
        {
            Title = "aaaaaaaaa",
            Body = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            AuthorId = Guid.Empty,
            BlogId = Guid.Empty,
        };

        // Act
        var response = await _serverFixture.PostAsync("api/blogs/posts", request, credentials: _userFromTenant1);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "'Blog' not found in database", "'Author' not found in database");
    }

    [FriendlyNamedFact("IT-P050"), Priority(50)]
    public async Task Domain_entity_from_a_proeprty_exists_in_database_and_belongs_to_tenant_restraint_should_be_validate_when_IDomainEntityValidator_is_implemented()
    {
        // Prepare
        var request = new CreatePost.Request
        {
            Title = "aaaaaaaaaa",
            Body = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            AuthorId = _author,
            BlogId = _blogFromTenant2,
        };

        // Act
        var response = await _serverFixture.PostAsync("api/blogs/posts", request, credentials: _userFromTenant1);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Possible unauthorized access");
    }

    [FriendlyNamedFact("IT-P060"), Priority(60)]
    public async Task Domain_database_restraints_should_not_return_errors_when_passing_and_IDomainEntityValidator_is_implemented()
    {

        // Prepare
        var request = new CreatePost.Request
        {
            Title = "aaaaaaaaaaa",
            Body = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            AuthorId = _author,
            BlogId = _blogFromTenant1,
        };

        // Act
        var response = await _serverFixture.PostAsync("api/blogs/posts", request, credentials: _userFromTenant1);

        // Assert the response
        response.ShouldBeSuccess();
    }


    [FriendlyNamedFact("IT-P070"), Priority(70)]
    public async Task Main_domain_entity_exists_in_database_and_belongs_to_tenant_restraint_should_be_validate_when_IDomainEntityValidator_is_implemented_in_update()
    {
        var post = _serverFixture.Context.Set<Post>().First();
        post.TenantId.ShouldBe("TENANT1");

        // Prepare
        var request = new UpdatePost.Request
        {
            Id = post.Id,
            Title = "bbbbbbbbb",
            Body = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
            AuthorId = _author,
            BlogId = _blogFromTenant1,
        };

        // Act
        var response = await _serverFixture.PutAsync("api/blogs/posts", request, credentials: _userFromTenant2);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Possible unauthorized access");
    }

    [FriendlyNamedFact("IT-P080"), Priority(80)]
    public async Task Main_domain_entity_exists_in_database_and_belongs_to_tenant_restraint_should_not_have_validate_errors_when_IDomainEntityValidator_is_implemented_in_update()
    {
        var post = _serverFixture.Context.Set<Post>().First();
        post.TenantId.ShouldBe("TENANT1");

        // Prepare
        var request = new UpdatePost.Request
        {
            Id = post.Id,
            Title = "bbbbbbbbb",
            Body = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
            AuthorId = _author,
            BlogId = _blogFromTenant1,
        };

        // Act
        var response = await _serverFixture.PutAsync("api/blogs/posts", request, credentials: _userFromTenant1);

        // Assert the response
        response.ShouldBeSuccess();
    }
}