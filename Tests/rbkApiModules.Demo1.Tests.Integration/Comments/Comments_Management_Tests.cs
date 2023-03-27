using System.Text.Json;
using rbkApiModules.Comments.Core;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Demo1.Tests.Integration.Comments;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class CommentsManagementTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public CommentsManagementTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Login call, just to store the access tokens for future tests to use
    /// </summary>
    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Login()
    {
        await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios");
    }

    /// <summary>
    /// User should not be able to create a comment without the comment
    /// </summary>
    /// DEPENDENCIES: none
    [InlineData(null)]
    [InlineData("")]
    [FriendlyNamedTheory("IT-301"), Priority(10)]
    public async Task User_Cannot_Create_Empty_Comment(string comment)
    {
        // Prepare
        var request = new CommentEntity.Request
        {
            EntityId = Guid.NewGuid(),
            ParentId = null,
            Comment = ""
        };

        // Act
        var response = await _serverFixture.PostAsync<TreeNode[]>("api/comments", request, authenticated: true);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Message' cannot be empty"); 
    }

    /// <summary>
    /// User should not be able to create a comment without being authenticated
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-302"), Priority(20)]
    public async Task User_Cannot_Create_Comment_Without_Being_Authenticated()
    {
        // Prepare
        var request = new CommentEntity.Request
        {
            EntityId = Guid.NewGuid(),
            ParentId = null,
            Comment = "This is my cool comment"
        };

        // Act
        var response = await _serverFixture.PostAsync<TreeNode[]>("api/comments", request, authenticated: false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// User should not be able to fetch comments without being authenticated
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-303"), Priority(30)]
    public async Task User_Cannot_Fetch_Comments_Without_Being_Authenticated()
    {
        // Act
        var response = await _serverFixture.GetAsync<TreeNode[]>($"api/comments/{Guid.NewGuid()}", authenticated: false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// User should not be able to create a child comment with non existant parent
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-304"), Priority(40)]
    public async Task User_Cannot_Create_Comment_Without_Non_Existant_Parent_Comment()
    {
        // Prepare
        var request = new CommentEntity.Request
        {
            EntityId = Guid.NewGuid(),
            ParentId = Guid.NewGuid(),
            Comment = "This is my cool comment"
        };

        // Act
        var response = await _serverFixture.PostAsync<TreeNode[]>("api/comments", request, authenticated: false);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// User should be able to create comments properly
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-305"), Priority(50)]
    public async Task User_Can_Create_Comments()
    {
        var entityA = new Guid("00000000-0000-0000-0000-00000000000a");
        var entityB = new Guid("00000000-0000-0000-0000-00000000000b");

        var admin1 = await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios");
        var admin2 = await _serverFixture.GetAccessTokenAsync("john.doe", "123", "buzios");
        var admin3 = await _serverFixture.GetAccessTokenAsync("jane.doe", "abc", "un-bs");

        async Task<Comment> CreateComment(int id, Guid entityId, Guid? parentId, string accessToken)
        {
            var request = new CommentEntity.Request
            {
                EntityId = entityId,
                ParentId = parentId,
                Comment = $"Comment {id}"
            };

            var response = await _serverFixture.PostAsync<TreeNode[]>("api/comments", request, accessToken);

            response.ShouldBeSuccess();
            response.Data.ShouldNotBeNull();

            var comment = _serverFixture.Context.Set<Comment>().First(x => x.Message == $"Comment {id}");

            return comment;
        }

        // ENTITY A
        //  - Comment 1 (john.doe)
        //  - Comment 2 (admin1)
        //     - Comment 3 (john.doe)
        //     - Comment 4 (john.doe)
        //        - Comment 6 (admin1)
        //        - Comment 7 (admin1)
        //        - Comment 8 (admin1)
        //     - Comment 5 (john.doe)
        // ENTITY B
        //  - Comment 9
        //     - Comment 11
        //        - Comment 12
        //           - Comment 13
        //  - Comment 14
        //  - Comment 10
        var comment1 = await CreateComment(1, entityA, null, admin2);
        var comment2 = await CreateComment(2, entityA, null, admin1);
        var comment9 = await CreateComment(9, entityB, null, admin3);
        var comment14 = await CreateComment(14, entityB, null, admin3);
        var comment10 = await CreateComment(10, entityB, null, admin3);
        var comment11 = await CreateComment(11, entityB, comment9.Id, admin3);
        var comment12 = await CreateComment(12, entityB, comment11.Id, admin3);
        var comment13 = await CreateComment(13, entityB, comment12.Id, admin3);
        var comment3 = await CreateComment(3, entityA, comment2.Id, admin2);
        var comment4 = await CreateComment(4, entityA, comment2.Id, admin2);
        var comment5 = await CreateComment(5, entityA, comment2.Id, admin2);
        var comment6 = await CreateComment(6, entityA, comment4.Id, admin1);
        var comment7 = await CreateComment(7, entityA, comment4.Id, admin1);
        var comment8 = await CreateComment(8, entityA, comment4.Id, admin1);

        _serverFixture.Context.Set<Comment>().Count().ShouldBe(14);
        _serverFixture.Context.Set<Comment>().Where(x => x.EntityId == entityA).Count().ShouldBe(8);
        _serverFixture.Context.Set<Comment>().Where(x => x.EntityId == entityB).Count().ShouldBe(6);
        _serverFixture.Context.Set<Comment>().Where(x => x.TenantId == "BUZIOS").Count().ShouldBe(8);
        _serverFixture.Context.Set<Comment>().Where(x => x.TenantId == "UN-BS").Count().ShouldBe(6);
        _serverFixture.Context.Set<Comment>().Where(x => x.ParentId == comment2.Id).Count().ShouldBe(3);
        _serverFixture.Context.Set<Comment>().Where(x => x.ParentId == comment4.Id).Count().ShouldBe(3);
        _serverFixture.Context.Set<Comment>().Where(x => x.ParentId == comment9.Id).Count().ShouldBe(1);
        _serverFixture.Context.Set<Comment>().Where(x => x.ParentId == comment11.Id).Count().ShouldBe(1);
        _serverFixture.Context.Set<Comment>().Where(x => x.ParentId == comment12.Id).Count().ShouldBe(1);


    }

    /// <summary>
    /// User should not be able to fetch comments 
    /// </summary>
    /// DEPENDENCIES: IT-305
    [FriendlyNamedFact("IT-306"), Priority(60)]
    public async Task User_Can_Fetch_Comments_For_Entity_A()
    {
        var token = await _serverFixture.GetAccessTokenAsync("john.doe", "123", "buzios");

        // Act
        var response = await _serverFixture.GetAsync<TreeNode[]>($"api/comments/00000000-0000-0000-0000-00000000000a", token);

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBe(2);
        response.Data[0].Children.Count.ShouldBe(0);
        response.Data[1].Children.Count.ShouldBe(3);
        response.Data[1].Children[0].Children.Count.ShouldBe(0);
        response.Data[1].Children[1].Children.Count.ShouldBe(3);
        response.Data[1].Children[0].Children.Count.ShouldBe(0);
        response.Data[1].Children[1].Children[0].Children.Count.ShouldBe(0);
        response.Data[1].Children[1].Children[1].Children.Count.ShouldBe(0);
        response.Data[1].Children[1].Children[2].Children.Count.ShouldBe(0);

        response.Data[0].Label.ShouldBe("Comment 1");
        response.Data[1].Label.ShouldBe("Comment 2"); 
        response.Data[1].Children[0].Label.ShouldBe("Comment 3");
        response.Data[1].Children[1].Label.ShouldBe("Comment 4");
        response.Data[1].Children[2].Label.ShouldBe("Comment 5");
        response.Data[1].Children[1].Children[0].Label.ShouldBe("Comment 6");
        response.Data[1].Children[1].Children[1].Label.ShouldBe("Comment 7");
        response.Data[1].Children[1].Children[2].Label.ShouldBe("Comment 8");

        ((JsonElement)response.Data[0].Data).GetProperty("displayName").GetString().ShouldBe("John Doe");
        ((JsonElement)response.Data[1].Data).GetProperty("displayName").GetString().ShouldBe("Administrador BUZIOS 1");
        ((JsonElement)response.Data[1].Children[0].Data).GetProperty("displayName").GetString().ShouldBe("John Doe");
        ((JsonElement)response.Data[1].Children[1].Data).GetProperty("displayName").GetString().ShouldBe("John Doe");
        ((JsonElement)response.Data[1].Children[2].Data).GetProperty("displayName").GetString().ShouldBe("John Doe");
        ((JsonElement)response.Data[1].Children[1].Children[0].Data).GetProperty("displayName").GetString().ShouldBe("Administrador BUZIOS 1");
        ((JsonElement)response.Data[1].Children[1].Children[1].Data).GetProperty("displayName").GetString().ShouldBe("Administrador BUZIOS 1");
        ((JsonElement)response.Data[1].Children[1].Children[2].Data).GetProperty("displayName").GetString().ShouldBe("Administrador BUZIOS 1");

        ((JsonElement)response.Data[0].Data).GetProperty("username").GetString().ShouldBe("john.doe");
        ((JsonElement)response.Data[1].Data).GetProperty("username").GetString().ShouldBe("admin1");
        ((JsonElement)response.Data[1].Children[0].Data).GetProperty("username").GetString().ShouldBe("john.doe");
        ((JsonElement)response.Data[1].Children[1].Data).GetProperty("username").GetString().ShouldBe("john.doe");
        ((JsonElement)response.Data[1].Children[2].Data).GetProperty("username").GetString().ShouldBe("john.doe");
        ((JsonElement)response.Data[1].Children[1].Children[0].Data).GetProperty("username").GetString().ShouldBe("admin1");
        ((JsonElement)response.Data[1].Children[1].Children[1].Data).GetProperty("username").GetString().ShouldBe("admin1");
        ((JsonElement)response.Data[1].Children[1].Children[2].Data).GetProperty("username").GetString().ShouldBe("admin1");

        ((JsonElement)response.Data[0].Data).GetProperty("avatar").GetString().Length.ShouldBeGreaterThan(50);
        ((JsonElement)response.Data[1].Data).GetProperty("avatar").GetString().Length.ShouldBeGreaterThan(50);
        ((JsonElement)response.Data[1].Children[0].Data).GetProperty("avatar").GetString().Length.ShouldBeGreaterThan(50);
        ((JsonElement)response.Data[1].Children[1].Data).GetProperty("avatar").GetString().Length.ShouldBeGreaterThan(50);
        ((JsonElement)response.Data[1].Children[2].Data).GetProperty("avatar").GetString().Length.ShouldBeGreaterThan(50);
        ((JsonElement)response.Data[1].Children[1].Children[0].Data).GetProperty("avatar").GetString().Length.ShouldBeGreaterThan(50);
        ((JsonElement)response.Data[1].Children[1].Children[1].Data).GetProperty("avatar").GetString().Length.ShouldBeGreaterThan(50);
        ((JsonElement)response.Data[1].Children[1].Children[2].Data).GetProperty("avatar").GetString().Length.ShouldBeGreaterThan(50);

        response.Data[0].Leaf.ShouldBe(true);
        response.Data[1].Leaf.ShouldBe(false);
        response.Data[1].Children[0].Leaf.ShouldBe(true);
        response.Data[1].Children[1].Leaf.ShouldBe(false);
        response.Data[1].Children[2].Leaf.ShouldBe(true);
        response.Data[1].Children[1].Children[0].Leaf.ShouldBe(true);
        response.Data[1].Children[1].Children[1].Leaf.ShouldBe(true);
        response.Data[1].Children[1].Children[2].Leaf.ShouldBe(true);

        response.Data[0].Label.ShouldBe("Comment 1");
        response.Data[1].Label.ShouldBe("Comment 2");
        response.Data[1].Children[0].Label.ShouldBe("Comment 3");
        response.Data[1].Children[1].Label.ShouldBe("Comment 4");
        response.Data[1].Children[2].Label.ShouldBe("Comment 5");
        response.Data[1].Children[1].Children[0].Label.ShouldBe("Comment 6");
        response.Data[1].Children[1].Children[1].Label.ShouldBe("Comment 7");
        response.Data[1].Children[1].Children[2].Label.ShouldBe("Comment 8");

        response.Data[0].Key.ShouldNotBeEmpty();
        response.Data[1].Key.ShouldNotBeEmpty();

        response.Data[0].Key.ShouldNotBeNull();
        response.Data[1].Key.ShouldNotBeNull();
    }

    /// <summary>
    /// User should be able to fetch comments 
    /// </summary>
    /// DEPENDENCIES: IT-305
    [FriendlyNamedFact("IT-307"), Priority(70)]
    public async Task User_Can_Fetch_Comments_For_Entity_B()
    {
        var token = await _serverFixture.GetAccessTokenAsync("jane.doe", "abc", "un-bs");

        // Act
        var response = await _serverFixture.GetAsync<TreeNode[]>($"api/comments/00000000-0000-0000-0000-00000000000b", token);

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBe(3);
        response.Data[0].Children.Count.ShouldBe(1);
        response.Data[1].Children.Count.ShouldBe(0);
        response.Data[2].Children.Count.ShouldBe(0);
        response.Data[0].Children[0].Children.Count.ShouldBe(1);
        response.Data[0].Children[0].Children[0].Children.Count.ShouldBe(1);
        response.Data[0].Children[0].Children[0].Children[0].Children.Count.ShouldBe(0);

        response.Data[0].Label.ShouldBe("Comment 9");
        response.Data[1].Label.ShouldBe("Comment 14");
        response.Data[2].Label.ShouldBe("Comment 10");
        response.Data[0].Children[0].Label.ShouldBe("Comment 11");
        response.Data[0].Children[0].Children[0].Label.ShouldBe("Comment 12");
        response.Data[0].Children[0].Children[0].Children[0].Label.ShouldBe("Comment 13");
    }
}