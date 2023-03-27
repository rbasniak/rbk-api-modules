using rbkApiModules.Faqs.Core;

namespace rbkApiModules.Tests.Integration.Faqs;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class FaqsQueryTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    private static string _superuser;
    private static string _user1;
    private static string _user2;

    public FaqsQueryTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Login call, just to store the access tokens for future tests to use
    /// </summary>
    [FriendlyNamedFact("IT-000"), Priority(-2)]
    public async Task Login()
    {
        _superuser = await _serverFixture.GetAccessTokenAsync("superuser", "admin", null);
        _user1 = await _serverFixture.GetAccessTokenAsync("admin1", "123", "buzios");
        _user2 = await _serverFixture.GetAccessTokenAsync("admin1", "123", "un-bs");
    }

    /// <summary>
    /// Prepare database for the tests
    /// </summary>
    [FriendlyNamedFact("IT-000"), Priority(-1)]
    public async Task Prepare()
    {
        var request1 = new CreateFaq.Request { Answer = "A1G", Question = "Q1G", Tag = "TAG1" };
        var request2 = new CreateFaq.Request { Answer = "A2G", Question = "Q2G", Tag = "TAG1" };
        var request3 = new CreateFaq.Request { Answer = "A1T1", Question = "Q1T1", Tag = "TAG1" };
        var request4 = new CreateFaq.Request { Answer = "A2T1", Question = "Q2T1", Tag = "TAG1" };
        var request5 = new CreateFaq.Request { Answer = "A1T2", Question = "Q1T2", Tag = "TAG1" };
        var request6 = new CreateFaq.Request { Answer = "A2T2", Question = "Q2T2", Tag = "TAG1" };
        var request7 = new CreateFaq.Request { Answer = "A3T2", Question = "Q3T2", Tag = "TAG1" };
        var request8 = new CreateFaq.Request { Answer = "Answer", Question = "Question", Tag = "TAG2" };

        async Task CreateFaq(CreateFaq.Request request, string token)
        {
            var response = await _serverFixture.PostAsync<FaqDetails>("api/faqs", request, token);
            response.ShouldBeSuccess();
        }

        await CreateFaq(request1, _superuser);
        await CreateFaq(request2, _superuser);
        await CreateFaq(request3, _user1);
        await CreateFaq(request4, _user1);
        await CreateFaq(request5, _user2);
        await CreateFaq(request6, _user2);
        await CreateFaq(request7, _user2);
        await CreateFaq(request8, _superuser);
    }

    /// <summary>
    /// Superuser should be able to see all faqs from all tenants
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-370"), Priority(10)]
    public async Task Superuser_Can_See_Faqs_From_All_Tenants()
    {
        // Act
        var response = await _serverFixture.GetAsync<FaqDetails[]>("api/faqs/TAG1", _superuser);

        // Assert the response
        response.ShouldBeSuccess(); 
        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBe(7);
        response.Data.Where(x => x.IsGlobal).Count().ShouldBe(2);
        response.Data.Where(x => x.Question == "Q1G" && x.Answer == "A1G" && x.IsGlobal == true).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q2G" && x.Answer == "A2G" && x.IsGlobal == true).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q1T1" && x.Answer == "A1T1" && x.IsGlobal == false).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q2T1" && x.Answer == "A2T1" && x.IsGlobal == false).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q1T2" && x.Answer == "A1T2" && x.IsGlobal == false).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q2T2" && x.Answer == "A2T2" && x.IsGlobal == false).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q3T2" && x.Answer == "A3T2" && x.IsGlobal == false).Count().ShouldBe(1);
    }

    /// <summary>
    /// User 1 should be able to see all global faqs plus its own
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-371"), Priority(20)]
    public async Task User1_Can_See_Global_Faqs_Plus_Their_Own()
    {
        // Act
        var response = await _serverFixture.GetAsync<FaqDetails[]>("api/faqs/TAG1", _user1);

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBe(4);
        response.Data.Where(x => x.IsGlobal).Count().ShouldBe(2);
        response.Data.Where(x => x.Question == "Q1G" && x.Answer == "A1G" && x.IsGlobal == true).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q2G" && x.Answer == "A2G" && x.IsGlobal == true).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q1T1" && x.Answer == "A1T1" && x.IsGlobal == false).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q2T1" && x.Answer == "A2T1" && x.IsGlobal == false).Count().ShouldBe(1);
    }

    /// <summary>
    /// User 2 should be able to see all global faqs plus its own
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-372"), Priority(30)]
    public async Task User2_Can_See_Global_Faqs_Plus_Their_Own()
    {
        // Act
        var response = await _serverFixture.GetAsync<FaqDetails[]>("api/faqs/TAG1", _user2);

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBe(5);
        response.Data.Where(x => x.IsGlobal).Count().ShouldBe(2);
        response.Data.Where(x => x.Question == "Q1G" && x.Answer == "A1G" && x.IsGlobal == true).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q2G" && x.Answer == "A2G" && x.IsGlobal == true).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q1T2" && x.Answer == "A1T2" && x.IsGlobal == false).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q2T2" && x.Answer == "A2T2" && x.IsGlobal == false).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Q3T2" && x.Answer == "A3T2" && x.IsGlobal == false).Count().ShouldBe(1);
    }

    /// <summary>
    /// User 2 should be able to see all global faqs plus its own
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-373"), Priority(40)]
    public async Task User2_Can_See_Global_Faqs_Plus_Their_Own_Using_Another_Tag()
    {
        // Act
        var response = await _serverFixture.GetAsync<FaqDetails[]>("api/faqs/TAG2", _user2);

        // Assert the response
        response.ShouldBeSuccess();
        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBe(1);
        response.Data.Where(x => x.IsGlobal).Count().ShouldBe(1);
        response.Data.Where(x => x.Question == "Question" && x.Answer == "Answer" && x.IsGlobal == true).Count().ShouldBe(1);
    }

    /// <summary>
    /// User should not be able to fetch faqs without a tag
    /// </summary>
    /// DEPENDENCIES: none
    [FriendlyNamedFact("IT-374"), Priority(50)]
    public async Task User_Cannot_Fetch_Faqs_Without_Tag()
    {
        // Act
        var response = await _serverFixture.GetAsync<FaqDetails[]>("api/faqs/", _user2);

        // Assert the response
        response.ShouldHaveErrors(HttpStatusCode.MethodNotAllowed);
    }
}