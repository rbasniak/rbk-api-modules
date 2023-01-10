namespace rbkApiModules.Tests.Integration
{
    public class ServerFixture: BaseServerFixture
    {
        public ServerFixture(): base(typeof(Startup))
        {

        }
    }
}
