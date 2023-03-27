﻿namespace rbkApiModules.Demo1.Tests.Integration.UiDefinitions;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class UiDefinitionTests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public UiDefinitionTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    /// <summary>
    /// UI Definitions test
    /// </summary>
    [FriendlyNamedFact("IT-19001"), Priority(1)]
    public async Task GetUiDefinitions()
    {
        var response = await _serverFixture.GetAsync("api/ui-definitions", credentials: null);

        response.ShouldBeSuccess();

        // TODO: add samples to be properly tested
    }
}