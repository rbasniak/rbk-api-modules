using rbkApiModules.Faqs.Core;

namespace rbkApiModules.Demo1.Tests.Integration.Faqs;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class FaqsManagementTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    private static string _superuser;
    private static string _user1;
    private static string _user2;

    public FaqsManagementTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Login call, just to store the access tokens for future tests to use
    /// </summary>
    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Login()
    {
        _superuser = await _serverFixture.GetAccessTokenAsync("superuser", "admin", null);
        _user1 = await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios");
        _user2 = await _serverFixture.GetAccessTokenAsync("admin1", "123", "un-bs");
    }

    /// <summary>
    /// Superuser should be able to create global faq
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-350"), Priority(10)]
    public async Task Superuser_Can_Create_Global_Faq()
    {
        // Prepare
        var request = new CreateFaq.Request
        {
            Tag = "SELLING-ORDERS",
            Question = "How to sell something?",
            Answer = "Force the customer to buy it"
        };

        // Act
        var response = await _serverFixture.PostAsync<FaqDetails>("api/faqs", request, _superuser);

        // Assert the response
        response.ShouldBeSuccess(); 
        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldNotBe(Guid.Empty.ToString());
        response.Data.Question.ShouldBe("How to sell something?");
        response.Data.Answer.ShouldBe("Force the customer to buy it");

        // Assert the database
        var faq = _serverFixture.Context.Set<Faq>().Find(new Guid(response.Data.Id));

        faq.ShouldNotBeNull();
        faq.Question.ShouldBe("How to sell something?");
        faq.Answer.ShouldBe("Force the customer to buy it");
        faq.Tag.ShouldBe("selling-orders");
        faq.HasNoTenant.ShouldBeTrue();
    }

    /// <summary>
    /// User should be able to create local faq
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-351"), Priority(20)]
    public async Task User_Can_Create_Local_Faq()
    {
        // Prepare
        var request = new CreateFaq.Request
        {
            Tag = "SELLING-ORDERS",
            Question = "How to convince the customer to buy something?",
            Answer = "Just force it!"
        };

        // Act
        var response = await _serverFixture.PostAsync<FaqDetails>("api/faqs", request, _user1);

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldNotBe(Guid.Empty.ToString());
        response.Data.Question.ShouldBe("How to convince the customer to buy something?");
        response.Data.Answer.ShouldBe("Just force it!");

        // Assert the database
        var faq = _serverFixture.Context.Set<Faq>().Find(new Guid(response.Data.Id));

        faq.ShouldNotBeNull();
        faq.Question.ShouldBe("How to convince the customer to buy something?");
        faq.Answer.ShouldBe("Just force it!");
        faq.Tag.ShouldBe("selling-orders");
        faq.HasTenant.ShouldBeTrue();
    }

    /// <summary>
    /// Superuser should not be able to update local faq
    /// </summary>
    /// DEPENDENCIES: IT-350
    [FriendlyNamedFact("IT-356"), Priority(30)]
    public async Task Superuser_Cannot_Update_Local_Faq()
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to convince the customer to buy something?");
        faq.HasTenant.ShouldBeTrue();

        var request = new UpdateFaq.Request
        {
            Id = faq.Id,
            Question = "How to sell something for real?",
            Answer = "Really force the customer to buy it"
        };

        // Act
        var response = await _serverFixture.PutAsync<FaqDetails>("api/faqs", request, _superuser);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Entity not found");
    }

    /// <summary>
    /// User should not be able to update global faq
    /// </summary>
    /// DEPENDENCIES: IT-351
    [FriendlyNamedFact("IT-357"), Priority(40)]
    public async Task User_Cannot_Update_Global_Faq()
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to sell something?");
        faq.HasNoTenant.ShouldBeTrue();

        var request = new UpdateFaq.Request
        {
            Id = faq.Id,
            Question = "How to convince the customer to buy something for real?",
            Answer = "Just force it, for real!"
        };

        // Act
        var response = await _serverFixture.PutAsync<FaqDetails>("api/faqs", request, _user1);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Entity not found");
    }

    /// <summary>
    /// Superuser should not be able to delete local faq
    /// </summary>
    /// DEPENDENCIES: IT-350
    [FriendlyNamedFact("IT-358"), Priority(50)]
    public async Task Superuser_Cannot_Delete_Local_Faq()
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to convince the customer to buy something?");
        faq.HasTenant.ShouldBeTrue(); 

        // Act
        var response = await _serverFixture.DeleteAsync($"api/faqs/{faq.Id}", _superuser);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Entity not found");
    }

    /// <summary>
    /// User should not be able to delete global faq
    /// </summary>
    /// DEPENDENCIES: IT-351
    [FriendlyNamedFact("IT-359"), Priority(60)]
    public async Task User_Cannot_Delete_Global_Faq()
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to sell something?");
        faq.HasNoTenant.ShouldBeTrue();

        // Act
        var response = await _serverFixture.DeleteAsync($"api/faqs/{faq.Id}", _user1);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Entity not found");
    }

    /// <summary>
    /// User should not be able to delete faq from another tenant
    /// </summary>
    /// DEPENDENCIES: IT-351
    [FriendlyNamedFact("IT-364"), Priority(70)]
    public async Task Superuser_Cannot_Delete_Faq_From_Another_Tenant()
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to convince the customer to buy something?");
        faq.HasTenant.ShouldBeTrue();

        // Act
        var response = await _serverFixture.DeleteAsync($"api/faqs/{faq.Id}", _user2);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Entity not found");
    }

    /// <summary>
    /// User should be able to update faq from another tenant
    /// </summary>
    /// DEPENDENCIES: IT-350
    [FriendlyNamedFact("IT-365"), Priority(80)]
    public async Task User_Can_Update_Faq_From_Another_Tenant()
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to convince the customer to buy something?");
        faq.HasTenant.ShouldBeTrue();

        var request = new UpdateFaq.Request
        {
            Id = faq.Id,
            Question = "Question",
            Answer = "Answer"
        };

        // Act
        var response = await _serverFixture.PutAsync<FaqDetails>("api/faqs", request, _user2);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Entity not found");
    }

    /// <summary>
    /// User should not be able to update local faq with empty question
    /// </summary>
    /// DEPENDENCIES: IT-351
    [InlineData("")]
    [InlineData(null)]
    [FriendlyNamedTheory("IT-365"), Priority(90)]
    public async Task User_Cannot_Update_Local_Faq_With_Empty_Question(string question)
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to convince the customer to buy something?");
        faq.HasTenant.ShouldBeTrue();

        var request = new UpdateFaq.Request
        {
            Id = faq.Id,
            Question = question,
            Answer = "Answer"
        };

        // Act
        var response = await _serverFixture.PutAsync<FaqDetails>("api/faqs", request, _user1);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Question' cannot be empty");
    }

    /// <summary>
    /// User should not be able to update local faq with empty answer
    /// </summary>
    /// DEPENDENCIES: IT-351
    [InlineData("")]
    [InlineData(null)]
    [FriendlyNamedTheory("IT-367"), Priority(100)]
    public async Task User_Cannot_Update_Local_Faq_With_Empty_Answer(string answer)
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to convince the customer to buy something?");
        faq.HasTenant.ShouldBeTrue();

        var request = new UpdateFaq.Request
        {
            Id = faq.Id,
            Question = "Question",
            Answer = answer
        };

        // Act
        var response = await _serverFixture.PutAsync<FaqDetails>("api/faqs", request, _user1);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Answer' cannot be empty");
    }

    /// <summary>
    /// Superuser should be able to update global faq
    /// </summary>
    /// DEPENDENCIES: IT-350
    [FriendlyNamedFact("IT-352"), Priority(110)]
    public async Task Superuser_Can_Update_Global_Faq()
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to sell something?");

        var request = new UpdateFaq.Request
        {
            Id = faq.Id,
            Question = "How to sell something for real?",
            Answer = "Really force the customer to buy it"
        };

        // Act
        var response = await _serverFixture.PutAsync<FaqDetails>("api/faqs", request, _superuser);

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldBe(faq.Id.ToString());
        response.Data.Question.ShouldBe("How to sell something for real?");
        response.Data.Answer.ShouldBe("Really force the customer to buy it");

        // Assert the database
        faq = _serverFixture.Context.Set<Faq>().Find(faq.Id);

        faq.ShouldNotBeNull();
        faq.Question.ShouldBe("How to sell something for real?");
        faq.Answer.ShouldBe("Really force the customer to buy it");
        faq.Tag.ShouldBe("selling-orders");
        faq.HasNoTenant.ShouldBeTrue();
    }

    /// <summary>
    /// User should be able to update local faq
    /// </summary>
    /// DEPENDENCIES: IT-351
    [FriendlyNamedFact("IT-353"), Priority(120)]
    public async Task User_Can_Update_Local_Faq()
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to convince the customer to buy something?");

        var request = new UpdateFaq.Request
        {
            Id = faq.Id,
            Question = "How to convince the customer to buy something for real?",
            Answer = "Just force it, for real!"
        };

        // Act
        var response = await _serverFixture.PutAsync<FaqDetails>("api/faqs", request, _user1);

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.Id.ShouldBe(faq.Id.ToString());
        response.Data.Question.ShouldBe("How to convince the customer to buy something for real?");
        response.Data.Answer.ShouldBe("Just force it, for real!");

        // Assert the database
        faq = _serverFixture.Context.Set<Faq>().Find(faq.Id);

        faq.ShouldNotBeNull();
        faq.Question.ShouldBe("How to convince the customer to buy something for real?");
        faq.Answer.ShouldBe("Just force it, for real!");
        faq.Tag.ShouldBe("selling-orders");
        faq.HasTenant.ShouldBeTrue();
    }


    /// <summary>
    /// Superuser should be able to delete global faq
    /// </summary>
    /// DEPENDENCIES: IT-352
    [FriendlyNamedFact("IT-354"), Priority(130)]
    public async Task Superuser_Can_Delete_Global_Faq()
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to sell something for real?");

        // Act
        var response = await _serverFixture.DeleteAsync($"api/faqs/{faq.Id}", _superuser);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        faq = _serverFixture.Context.Set<Faq>().Find(faq.Id);

        faq.ShouldBeNull();
    }

    /// <summary>
    /// User should be able to delete local faq
    /// </summary>
    /// DEPENDENCIES: IT-353
    [FriendlyNamedFact("IT-355"), Priority(140)]
    public async Task User_Can_Delete_Local_Faq()
    {
        // Prepare
        var faq = _serverFixture.Context.Set<Faq>().First(x => x.Question == "How to convince the customer to buy something for real?");

        // Act
        var response = await _serverFixture.DeleteAsync($"api/faqs/{faq.Id}", _user1);

        // Assert the response
        response.ShouldBeSuccess();

        // Assert the database
        faq = _serverFixture.Context.Set<Faq>().Find(faq.Id);

        faq.ShouldBeNull();
    }

    /// <summary>
    /// Superuser should not be able to create faq with empty tag
    /// </summary>
    /// DEPENDENCIES: none
    [InlineData(null)]
    [InlineData("")]
    [FriendlyNamedTheory("IT-360"), Priority(150)]
    public async Task Superuser_Cannot_Create_Global_Faq_With_Empty_Tag(string tag)
    {
        // Prepare
        var request = new CreateFaq.Request
        {
            Tag = tag,
            Question = "Question",
            Answer = "Answer"
        };

        // Act
        var response = await _serverFixture.PostAsync<FaqDetails>("api/faqs", request, _superuser);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Tag' cannot be empty");
    }

    /// <summary>
    /// Superuser should not be able to create faq with empty question
    /// </summary>
    /// DEPENDENCIES: none
    [InlineData(null)]
    [InlineData("")]
    [FriendlyNamedTheory("IT-361"), Priority(160)]
    public async Task Superuser_Cannot_Create_Global_Faq_With_Empty_Question(string question)
    {
        // Prepare
        var request = new CreateFaq.Request
        {
            Tag = "tag",
            Question = question,
            Answer = "Answer"
        };

        // Act
        var response = await _serverFixture.PostAsync<FaqDetails>("api/faqs", request, _user1);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Question' cannot be empty");
    }

    /// <summary>
    /// Superuser should not be able to create faq with empty answer
    /// </summary>
    /// DEPENDENCIES: none
    [InlineData(null)]
    [InlineData("")]
    [FriendlyNamedTheory("IT-362"), Priority(170)]
    public async Task Superuser_Cannot_Create_Global_Faq_With_Empty_Answer(string answer)
    {
        // Prepare
        var request = new CreateFaq.Request
        {
            Tag = "tag",
            Question = "Question",
            Answer = answer
        };

        // Act
        var response = await _serverFixture.PostAsync<FaqDetails>("api/faqs", request, _user1);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "The field 'Answer' cannot be empty");
    }

    /// <summary>
    /// Superuser should not be able to delete faq that does not exist
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-363"), Priority(180)]
    public async Task Superuser_Cannot_Delete_Faq_That_Does_Not_Exist()
    {
        // Act
        var response = await _serverFixture.DeleteAsync($"api/faqs/{Guid.NewGuid()}", _superuser);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Entity not found");
    }
}