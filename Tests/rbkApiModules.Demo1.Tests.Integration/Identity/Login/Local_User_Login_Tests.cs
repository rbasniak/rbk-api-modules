using rbkApiModules.Commons.Core;
using rbkApiModules.Testing.Core;
using System.IdentityModel.Tokens.Jwt;

namespace rbkApiModules.Demo1.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class LocalUserLoginTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public LocalUserLoginTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    #region Expected databse structure

    /// BUZIOS
    ///   - John Doe
    ///     - Employee
    ///       - CAN_GENERATE_REPORTS
    ///     * ALLOW: CAN_SHARE_REPORTS
    ///   - Jane Doe
    ///     - Employee
    ///       - CAN_GENERATE_REPORTS
    ///     - Manager (application wide)
    ///       - CAN_APROVE_REPORTS
    /// 
    /// UN-ES
    ///   - John Doe
    ///     - Manager (tenant wide)
    ///       - CAN_SHARE_REPORTS
    ///       - CAN_APROVE_REPORTS
    ///   - Jane Doe
    ///     - Manager (tenant wide)
    ///       - CAN_SHARE_REPORTS
    ///       - CAN_APROVE_REPORTS
    ///     * BLOCK: CAN_SHARE_REPORTS

    #endregion

    /// <summary>
    /// User cannot log with wrong password
    /// </summary>
    [FriendlyNamedFact("IT-120"), Priority(10)]
    public async Task User_Cannot_Login_With_Wrong_Password()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "john.doe",
            Password = "321",
            Tenant = "buzios"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Invalid credentials");
    }

    /// <summary>
    /// User cannot log with in the wrong tenant
    /// </summary>
    [FriendlyNamedFact("IT-121"), Priority(15)]
    public async Task User_Cannot_Login_With_Wrong_Tenant()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "john.doe",
            Password = "123",
            Tenant = "xxxxx"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Invalid credentials");
    }

    /// <summary>
    /// The ACME tenant exists in the databse, try to login with the global admin and this tenant.
    /// Since the global admin can only manage tenants, it should not be able to login with 
    /// a given tenant
    /// </summary>
    [FriendlyNamedFact("IT-125"), Priority(20)]
    public async Task User_Cannot_Login_With_Right_Credentials_But_Wrong_Tenant()
    {
        // Prepare 
        var request = new UserLogin.Request
        {
            Username = "john.doe",
            Password = "123",
            Tenant = "UN_ES"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Invalid credentials");
    }

    /// <summary>
    /// Before the user logins, it must not have a refresh token, it's created during the first login
    /// But after it's been created, it must be the same in subsequent request and have a validity
    /// </summary>
    [FriendlyNamedFact("IT-126"), Priority(30)]
    public async Task After_User_Login_It_Must_Have_a_Refresh_Token_And_The_Same_In_Subsequent_Logins()
    {
        // Precondition checks
        var user = _serverFixture.Context.Set<User>().Single(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");
        user.RefreshToken.ShouldBeNull();
        user.LastLogin.ShouldBeNull();

        // Prepare
        var request = new UserLogin.Request
        {
            Username = "john.doe",
            Password = "123",
            Tenant = "buzios"
        };

        // Act
        var response1 = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert the response
        response1.ShouldBeSuccess();

        response1.Data.ShouldNotBeNull();
        response1.Data.AccessToken.ShouldNotBeNull();
        response1.Data.RefreshToken.ShouldNotBeNull();

        // Assert the database
        user = _serverFixture.Context.Set<User>().Single(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");
        user.RefreshToken.ShouldBe(response1.Data.RefreshToken);
        user.RefreshTokenValidity.ShouldBeGreaterThan(DateTime.UtcNow);
        user.LastLogin.ShouldNotBeNull();
        user.LastLogin.Value.ShouldBeLessThan(DateTime.UtcNow);

        var validity = user.RefreshTokenValidity;
        var lastLogin = user.LastLogin;

        Thread.Sleep(1500); // Needed so the access token data can actually change between the two calls

        // Act again
        var response2 = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert the response
        response2.ShouldBeSuccess();

        response2.Data.ShouldNotBeNull();
        response2.Data.AccessToken.ShouldNotBeNull();
        response2.Data.AccessToken.ShouldNotBe(response1.Data.AccessToken);
        response2.Data.RefreshToken.ShouldBe(response1.Data.RefreshToken);

        // Assert the database
        user = _serverFixture.Context.Set<User>().Single(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");
        user.RefreshToken.ShouldBe(response2.Data.RefreshToken);
        user.RefreshTokenValidity.ShouldBeGreaterThan(DateTime.UtcNow);
        user.RefreshTokenValidity.ShouldBe(validity);
        user.LastLogin.ShouldNotBeNull();
        user.LastLogin.Value.ShouldBeLessThan(DateTime.UtcNow);
        user.LastLogin.ShouldNotBe(lastLogin);
    }

    /// <summary>
    /// User john.doe can login with right credentials, in the BUZIOS tenant, and have the right claims
    /// </summary>
    [FriendlyNamedFact("IT-124A"), Priority(40)]
    public async Task User1_Can_Login_With_Username()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "john.doe",
            Password = "123",
            Tenant = "buzios"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.AccessToken.ShouldNotBeNull();
        response.Data.RefreshToken.ShouldNotBeNull();

        var handler = new JwtSecurityTokenHandler();
        var tokenData = handler.ReadJwtToken(response.Data.AccessToken);

        var tenant = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Tenant).Value;
        var username1 = tokenData.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
        var username2 = tokenData.Claims.First(claim => claim.Type == System.Security.Claims.ClaimTypes.Name).Value;
        var displayName = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.DisplayName).Value;
        var avatar = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Avatar).Value;
        var roles = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.Roles).ToList();
        var allowedTenants = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.AllowedTenants).ToList();
        var authenticationMode = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.AuthenticationMode).Value;

        tenant.ShouldBe("BUZIOS");
        username1.ShouldBe("john.doe");
        username2.ShouldBe("john.doe");
        displayName.ShouldBe("John Doe");
        avatar.ShouldStartWith("data:image/svg+xml;base64,");
        roles.Count.ShouldBe(2);
        roles.FirstOrDefault(x => x.Value == "CAN_GENERATE_REPORTS").ShouldNotBeNull();
        roles.FirstOrDefault(x => x.Value == "CAN_SHARE_REPORTS").ShouldNotBeNull();
        allowedTenants.Count.ShouldBe(1);
        allowedTenants.FirstOrDefault(x => x.Value == "BUZIOS").ShouldNotBeNull();
        authenticationMode.ShouldBe("Credentials");
    }

    /// <summary>
    /// User jane.doe can login with right credentials, in the BUZIOS tenant, and have the right claims
    /// </summary>
    [FriendlyNamedFact("IT-124B"), Priority(50)]
    public async Task User2_Can_Login_With_Username()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "jane.doe",
            Password = "123",
            Tenant = "buzios"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.AccessToken.ShouldNotBeNull();
        response.Data.RefreshToken.ShouldNotBeNull();

        var handler = new JwtSecurityTokenHandler();
        var tokenData = handler.ReadJwtToken(response.Data.AccessToken);

        var tenant = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Tenant).Value;
        var username1 = tokenData.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
        var username2 = tokenData.Claims.First(claim => claim.Type == System.Security.Claims.ClaimTypes.Name).Value;
        var displayName = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.DisplayName).Value;
        var avatar = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Avatar).Value;
        var roles = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.Roles).ToList();

        tenant.ShouldBe("BUZIOS");
        username1.ShouldBe("jane.doe");
        username2.ShouldBe("jane.doe");
        displayName.ShouldBe("Jane Doe");
        avatar.ShouldStartWith("data:image/svg+xml;base64,");
        roles.Count.ShouldBe(2);
        roles.FirstOrDefault(x => x.Value == "CAN_APPROVE_REPORTS").ShouldNotBeNull();
        roles.FirstOrDefault(x => x.Value == "CAN_GENERATE_REPORTS").ShouldNotBeNull();
    }

    /// <summary>
    /// User john.doe can login with right credentials, in the UN-ES tenant, and have the right claims
    /// </summary>
    [FriendlyNamedFact("IT-124C"), Priority(60)]
    public async Task User3_Can_Login_With_Username()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "john.doe",
            Password = "abc",
            Tenant = "un-bs"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.AccessToken.ShouldNotBeNull();
        response.Data.RefreshToken.ShouldNotBeNull();

        var handler = new JwtSecurityTokenHandler();
        var tokenData = handler.ReadJwtToken(response.Data.AccessToken);

        var tenant = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Tenant).Value;
        var username1 = tokenData.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
        var username2 = tokenData.Claims.First(claim => claim.Type == System.Security.Claims.ClaimTypes.Name).Value;
        var displayName = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.DisplayName).Value;
        var avatar = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Avatar).Value;
        var roles = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.Roles).ToList();

        tenant.ShouldBe("UN-BS");
        username1.ShouldBe("john.doe");
        username2.ShouldBe("john.doe");
        displayName.ShouldBe("John Doe");
        avatar.ShouldStartWith("data:image/svg+xml;base64,");
        roles.Count.ShouldBe(2);
        roles.FirstOrDefault(x => x.Value == "CAN_APPROVE_REPORTS").ShouldNotBeNull();
        roles.FirstOrDefault(x => x.Value == "CAN_SHARE_REPORTS").ShouldNotBeNull();
    }

    /// <summary>
    /// User jane.doe can login with right credentials, in the UN-ES tenant, and have the right claims
    /// </summary>
    [FriendlyNamedFact("IT-124C"), Priority(70)]
    public async Task User4_Can_Login_With_Username()
    {
        // Prepare
        var request = new UserLogin.Request
        {
            Username = "jane.doe",
            Password = "abc",
            Tenant = "un-bs"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", request, credentials: null);

        // Assert
        response.ShouldBeSuccess();

        response.Data.ShouldNotBeNull();
        response.Data.AccessToken.ShouldNotBeNull();
        response.Data.RefreshToken.ShouldNotBeNull();

        var handler = new JwtSecurityTokenHandler();
        var tokenData = handler.ReadJwtToken(response.Data.AccessToken);

        var tenant = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Tenant).Value;
        var username1 = tokenData.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
        var username2 = tokenData.Claims.First(claim => claim.Type == System.Security.Claims.ClaimTypes.Name).Value;
        var displayName = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.DisplayName).Value;
        var avatar = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Avatar).Value;
        var roles = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.Roles).ToList();

        tenant.ShouldBe("UN-BS");
        username1.ShouldBe("jane.doe");
        username2.ShouldBe("jane.doe");
        displayName.ShouldBe("Jane Doe");
        avatar.ShouldStartWith("data:image/svg+xml;base64,");
        roles.Count.ShouldBe(1);
        roles.FirstOrDefault(x => x.Value == "CAN_APPROVE_REPORTS").ShouldNotBeNull();
    }

    /// <summary>
    /// User cannot refresh token with empty data
    /// </summary>
    [InlineData("")]
    [InlineData(null)]
    [FriendlyNamedTheory("IT-145"), Priority(80)]
    public async Task User_Cannot_Refresh_Token_With_Empty_Data(string refreshToken)
    {
        // Act
        var request = new RenewAccessToken.Request
        {
            RefreshToken = refreshToken
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/refresh-token", request, credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Refresh token' cannot be empty");
    }

    /// <summary>
    /// User cannot refresh token with a token that does not exist
    /// </summary>
    [FriendlyNamedFact("IT-147"), Priority(90)]
    public async Task User_Cannot_Refresh_Token_With_Non_Existent_Token()
    {
        // Act
        var request = new RenewAccessToken.Request
        {
            RefreshToken = "non-existent-token"
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/refresh-token", request, credentials: null);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Refresh token does not exist anymore");
    }

    /// <summary>
    /// User can refresh token with valid data
    /// </summary>
    [FriendlyNamedFact("IT-146/IT-216"), Priority(100)]
    public async Task User_Can_Refresh_Token_With_Valid_Data()
    {
        // Prepare
        var loginRequest = new UserLogin.Request
        {
            Username = "jane.doe",
            Password = "abc",
            Tenant = "un-bs"
        };
        var authDataResponse = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", loginRequest, credentials: null);

        authDataResponse.ShouldBeSuccess();

        // To allow for the token data to change between calls
        Thread.Sleep(1500);

        // Act
        var request = new RenewAccessToken.Request
        {
            RefreshToken = authDataResponse.Data.RefreshToken
        };

        // Act
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/refresh-token", request, credentials: null);

        // Assert
        response.ShouldBeSuccess();
        response.Data.AccessToken.ShouldNotBe(authDataResponse.Data.AccessToken);

        var handler = new JwtSecurityTokenHandler();
        var tokenData = handler.ReadJwtToken(response.Data.AccessToken);

        var lastLoginClaim = tokenData.Claims.FirstOrDefault(claim => claim.Type == "last-login");

        lastLoginClaim.ShouldNotBeNull();

        var tenant = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Tenant).Value;
        var username1 = tokenData.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
        var username2 = tokenData.Claims.First(claim => claim.Type == System.Security.Claims.ClaimTypes.Name).Value;
        var displayName = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.DisplayName).Value;
        var avatar = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.Avatar).Value;
        var roles = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.Roles).ToList();
        var allowedTenants = tokenData.Claims.Where(claim => claim.Type == JwtClaimIdentifiers.AllowedTenants).ToList();
        var authenticationMode = tokenData.Claims.First(claim => claim.Type == JwtClaimIdentifiers.AuthenticationMode).Value;

        tenant.ShouldBe("UN-BS");
        username1.ShouldBe("jane.doe");
        username2.ShouldBe("jane.doe");
        displayName.ShouldBe("Jane Doe");
        avatar.ShouldStartWith("data:image/svg+xml;base64,");
        roles.Count.ShouldBe(1);
        roles.FirstOrDefault(x => x.Value == "CAN_APPROVE_REPORTS").ShouldNotBeNull();
        allowedTenants.Count.ShouldBe(1);
        allowedTenants.FirstOrDefault(x => x.Value == "UN-BS").ShouldNotBeNull();
        authenticationMode.ShouldBe("Credentials");
    }

    /// <summary>
    /// When the user logins, it receives a refresh token, to get a new access token when it expires.
    /// The refresh token has an expiration itself. If it's expired, the user cannot generate a new 
    /// access token
    /// </summary>
    [FriendlyNamedFact("IT-148"), Priority(110)]
    public async Task User_Cannot_Request_New_Access_Token_If_Refresh_Token_Is_Expired()
    {
        // Precondition checks
        var user = _serverFixture.Context.Set<User>().Single(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");

        // Prepare
        var loginRequest = new UserLogin.Request
        {
            Username = "john.doe",
            Password = "123",
            Tenant = "buzios"
        };
        var loginResponse = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", loginRequest, credentials: null);
        loginResponse.ShouldBeSuccess();

        Thread.Sleep(7500); // Needed so the refresh token expires (6s, setup in the appsettings.Testing.json)

        user.RefreshTokenValidity.ShouldBeLessThan(DateTime.UtcNow);

        // Act 
        var request = new RenewAccessToken.Request
        {
            RefreshToken = user.RefreshToken
        };
        var response = await _serverFixture.PostAsync<JwtResponse>("api/authentication/refresh-token", request, credentials: null);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Refresh token expired");
    }

    [FriendlyNamedFact("IT-???"), Priority(120)]
    public async Task User_Cannot_Login_Using_Windows_Authentication()
    {
        // Precondition checks
        var user = _serverFixture.Context.Set<User>().Single(x => x.Username == "john.doe" && x.TenantId == "BUZIOS");

        // Prepare
        var loginRequest = new UserLogin.Request
        {
            Tenant = "buzios"
        };
        var loginResponse = await _serverFixture.PostAsync<JwtResponse>("api/authentication/login", loginRequest, credentials: Environment.UserName);
        loginResponse.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'User' cannot be empty");
    }
}

