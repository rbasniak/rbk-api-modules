namespace rbkApiModules.Tests.Integration.CodeGeneration;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class CodeGenerationTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public CodeGenerationTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// Smoke test for code generation
    /// </summary>
    [FriendlyNamedFact("IT-19000"), Priority(1)]
    public async Task CallCodeGeneration()
    {
        var response = await _serverFixture.GetAsync("api/code-generator?directUpdate=false", credentials: null);

        response.ShouldBeSuccess();

        var remainingFiles = Directory.GetFiles(Path.Combine(_serverFixture.ContentFolder, "wwwroot", "_temp"));

        remainingFiles.Length.ShouldBe(0);
    }
}