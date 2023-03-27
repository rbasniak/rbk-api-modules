using Demo1.Api;

namespace rbkApiModules.Demo1.Tests.Integration;

public class ServerFixture: BaseServerFixture
{
    public ServerFixture(): base(typeof(Startup), AuthenticationMode.Credentials)
    {

    }
}
