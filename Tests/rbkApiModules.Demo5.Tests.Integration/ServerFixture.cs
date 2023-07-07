using Demo5;

namespace rbkApiModules.Demo5.Tests.Integration;

public class ServerFixture: BaseServerFixture
{
    public ServerFixture(): base(typeof(Startup), "credentials")
    {

    }
}
