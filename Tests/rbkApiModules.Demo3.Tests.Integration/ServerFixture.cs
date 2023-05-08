using Demo3;

namespace rbkApiModules.Demo3.Tests.Integration;

public class ServerFixture: BaseServerFixture
{
    public ServerFixture(): base(typeof(Startup), "windows")
    {

    }
}
