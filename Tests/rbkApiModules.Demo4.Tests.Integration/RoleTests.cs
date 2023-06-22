using rbkApiModules.Commons.Core;
using rbkApiModules.Identity.Core.DataTransfer.Roles;
using System.IdentityModel.Tokens.Jwt;

namespace rbkApiModules.Demo4.Tests.Integration;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class RoleTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public RoleTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    [FriendlyNamedFact("IT-0000"), Priority(-1)]
    public async Task Seed()
    {
        await _serverFixture.GetAccessTokenAsync("superuser", "admin", null);
    }

    /// <summary>
    /// With Windows Authentication and allow user creation on login, user cannot rename the default role
    /// </summary>
    [FriendlyNamedFact("IT-R001"), Priority(10)]
    public async Task User_cannot_rename_role_if_it_is_the_default_role()
    {
        var role = await _serverFixture.Context.Set<Role>().FirstAsync(x => x.Name == "Readonly user" && x.TenantId == null);

        var users = await _serverFixture.Context.Set<User>().ToListAsync();

        // Act
        var response = await _serverFixture.PutAsync<RoleDetails>("api/authorization/roles", new RenameRole.Request { Id = role.Id, Name = "Renamed role" }, true);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Cannot rename the default user role when the application allow for automatic user creation");
    }

    /// <summary>
    /// With Windows Authentication and allow user creation on login, user cannot delete the default role
    /// </summary>
    [FriendlyNamedFact("IT-R002"), Priority(20)]
    public async Task User_cannot_delete_role_if_it_is_the_default_role()
    {
        var role = await _serverFixture.Context.Set<Role>().FirstAsync(x => x.Name == "Readonly user" && x.TenantId == null);

        var users = await _serverFixture.Context.Set<User>().ToListAsync();

        // Act
        var response = await _serverFixture.DeleteAsync($"api/authorization/roles/{role.Id}", true);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Cannot delete the default user role when the application allow for automatic user creation");
    }
}