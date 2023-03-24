namespace rbkApiModules.Tests.Integration
{
    public class NtlmServerFixture : BaseServerFixture
    {
        public NtlmServerFixture(): base(typeof(Startup), AuthenticationMode.Windows)
        {

        }
    }
}
