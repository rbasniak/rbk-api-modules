using Demo4;

namespace rbkApiModules.Demo4.Tests.Integration;

public class ServerFixture: BaseServerFixture
{
    public ServerFixture(): base(typeof(Startup), "windows")
    {

    }
}
