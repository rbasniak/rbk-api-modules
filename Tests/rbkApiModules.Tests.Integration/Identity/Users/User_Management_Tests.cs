namespace rbkApiModules.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UserManagementTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UserManagementTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Login call, just to store the access tokens for future tests to use
    /// </summary>
    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Login()
    {
        // To register the LastLogin property
        await _serverFixture.GetAccessTokenAsync("john.doe", "123", "buzios");
        await _serverFixture.GetAccessTokenAsync("jane.doe", "123", "buzios");
    }

    /// <summary>
    /// The global admin should be able to create a new tenant called ACME
    /// </summary>
    [FriendlyNamedFact("IT-140"), Priority(10)]
    public async Task Global_Admin_Cannot_View_List_Of_Users()
    {
        // Act
        var response = await _serverFixture.GetAsync<UserDetails>("api/authorization/users", await _serverFixture.GetAccessTokenAsync("superuser", "admin", null));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// A local admin should be able to see the list of their users 
    /// DEPENDENCIES: Seed data
    /// </summary>
    [FriendlyNamedFact("IT-141"), Priority(20)]
    public async Task Local_Admin_Cannot_View_List_Of_Users()
    {
        // Act
        var response = await _serverFixture.GetAsync<UserDetails[]>("api/authorization/users", await _serverFixture.GetAccessTokenAsync("admin1", "123", "Buzios"));

        // Assert the response
        response.ShouldBeSuccess();

        var userJohn = response.Data.FirstOrDefault(x => x.Username == "john.doe");
        var userJane = response.Data.FirstOrDefault(x => x.Username == "jane.doe");

        userJohn.ShouldNotBeNull();
        userJohn.Roles.Count().ShouldBe(1);
        userJohn.LastLogin.ShouldNotBeNull();
        userJohn.Roles[0].Name.ShouldBe("Employee");

        userJane.ShouldNotBeNull();
        userJane.LastLogin.ShouldNotBeNull();
        userJane.Roles.Count().ShouldBe(2);
        userJane.Roles.FirstOrDefault(x => x.Name == "Employee").ShouldNotBeNull();
        userJane.Roles.FirstOrDefault(x => x.Name == "Manager").ShouldNotBeNull();
    }

    /// <summary>
    /// A local admin should be able to set the user's list of roles
    /// DEPENDENCIES: Seed data
    /// </summary>
    [FriendlyNamedFact("IT-170"), Priority(30)]
    public async Task Local_Admin_Can_Set_List_Of_Roles()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(0);

        var role1 = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Employee" && x.TenantId == null);
        var role2 = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Manager" && x.TenantId == null);

        var request = new ReplaceUserRoles.Request 
        { 
            RoleIds = new[] { role1.Id, role2.Id } ,
            Username = user.Username
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/set-roles", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "un-bs"));

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Roles.Length.ShouldBe(2);
        response.Data.Roles.SingleOrDefault(x => x.Id == role1.Id.ToString()).ShouldNotBeNull();
        response.Data.Roles.SingleOrDefault(x => x.Id == role2.Id.ToString()).ShouldNotBeNull();

        // Assert the database
        user = _serverFixture.Context.Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(2);
        user.Roles.SingleOrDefault(x => x.RoleId == role1.Id).ShouldNotBeNull();
        user.Roles.SingleOrDefault(x => x.RoleId == role2.Id).ShouldNotBeNull();
    }

    /// <summary>
    /// A local admin should be able to set the user's list of roles using an empty array, thus removing all user's access
    /// DEPENDENCIES: IT-170
    /// </summary>
    [FriendlyNamedFact("IT-175"), Priority(35)]
    public async Task Local_Admin_Can_Remove_All_Roles_From_User()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(2);

        var request = new ReplaceUserRoles.Request
        {
            RoleIds = new Guid[0],
            Username = user.Username
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/set-roles", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "un-bs"));

        // Assert the response
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.Roles.Length.ShouldBe(0);

        // Assert the database
        user = _serverFixture.Context.Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(0);
    }

    /// <summary>
    /// A local admin should not be able to set the user's list of roles without user
    /// DEPENDENCIES: Seed data
    /// </summary>
    [InlineData("")]
    [InlineData(null)]
    [FriendlyNamedTheory("IT-172"), Priority(40)]
    public async Task Local_Admin_Cannot_Set_List_Of_Roles_With_Empty_User(string username)
    {
        // Prepare
        var role1 = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Employee" && x.TenantId == null);
        var role2 = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Manager" && x.TenantId == null);

        var request = new ReplaceUserRoles.Request
        {
            RoleIds = new[] { role1.Id, role2.Id },
            Username = username
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/set-roles", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "un-bs"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "O campo 'User' não pode ser vazio");
    }

    /// <summary>
    /// A local admin should not be able to set the user's list of roles if the user does not exist
    /// DEPENDENCIES: Seed data
    /// </summary>
    [FriendlyNamedFact("IT-173"), Priority(50)]
    public async Task Local_Admin_Cannot_Set_List_Of_Roles_With_Non_Existant_User()
    {
        // Prepare
        var role1 = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Employee" && x.TenantId == null);
        var role2 = _serverFixture.Context.Set<Role>().Single(x => x.Name == "Manager" && x.TenantId == null);

        var request = new ReplaceUserRoles.Request
        {
            RoleIds = new[] { role1.Id, role2.Id },
            Username = "tony.stark"
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/set-roles", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "User not found.");
    }

    /// <summary>
    /// A local admin should be able to set the user's list of roles using invalid roles
    /// DEPENDENCIES: Seed data
    /// </summary>
    [FriendlyNamedFact("IT-174"), Priority(60)]
    public async Task Local_Admin_Cannot_Set_List_Of_Roles_With_Invalid_Roles()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(0);

        var request = new ReplaceUserRoles.Request
        {
            RoleIds = new[] { Guid.NewGuid() },
            Username = user.Username
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/set-roles", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "un-bs"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Não foi possível localizar o role no servidor");
    }

    /// <summary>
    /// A local admin should not be able to set the user's roles with a null list
    /// DEPENDENCIES: Seed data
    /// </summary>
    [FriendlyNamedFact("IT-176"), Priority(70)]
    public async Task Local_Admin_Cannot_Set_List_Of_Roles_With_Null_List()
    {
        // Prepare
        var user = _serverFixture.Context.Set<User>().Include(x => x.Roles).SingleOrDefault(x => x.Username == "tony.stark" && x.TenantId == "UN-BS");
        user.ShouldNotBeNull();
        user.Roles.ToList().Count().ShouldBe(0);

        var request = new ReplaceUserRoles.Request
        {
            RoleIds = null,
            Username = user.Username
        };

        // Act
        var response = await _serverFixture.PostAsync<UserDetails>("api/authorization/users/set-roles", request, await _serverFixture.GetAccessTokenAsync("admin1", "123", "un-bs"));

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The list of roles must be provided");
    }
}
