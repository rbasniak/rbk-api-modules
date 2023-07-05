using Demo3.Commands;
using Demo3.Models;
using rbkApiModules.Identity.Core.DataTransfer.Users;
using System.Diagnostics;

namespace rbkApiModules.Demo3.Tests.Integration.Identity;

[TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
public class Project_Creation_Tests : SequentialTest, IClassFixture<ServerFixture>
{
    private ServerFixture _serverFixture;

    public Project_Creation_Tests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    [FriendlyNamedFact("Proj-000"), Priority(-1)]
    public async Task SeedProject()
    {
        var context = _serverFixture.Context;

        context.Set<Project>().Add(new Project("proj1", "code1", "mdb1"));

        context.SaveChanges();

    }

    /// <summary>
    /// User cannot be created if username is not supplied
    /// DEPENDENCIES: none
    /// </summary>
    [FriendlyNamedFact("proj-001"), Priority(10)]
    public async Task Project_can_be_created()
    {
        // Prepare
        var request = new CreateProject.Request
        {
            Name = "Projeto2",
            Code = "Code2",
            Mdb = "MDB2",
        };

        // Act
        var response = await _serverFixture.PostAsync<ProjectDto.Details>("api/projects", request, false);
        response.ShouldBeSuccess();
    }
}
