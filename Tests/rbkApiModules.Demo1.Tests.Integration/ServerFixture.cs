using Demo1.Api;

namespace rbkApiModules.Tests.Integration;

public class ServerFixture: BaseServerFixture
{
    public ServerFixture(): base(typeof(Startup), AuthenticationMode.Credentials)
    {

    }
}
