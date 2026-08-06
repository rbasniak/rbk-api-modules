namespace Demo.MultipleApis.Tests.Infrastructure;

public abstract class MultiApiTestingServerBase<TProgram> : RbkTestingServer<TProgram> where TProgram : class
{
    protected abstract string ConfigFolderName { get; }

    protected override string GetConfigurationBasePath()
        => Path.Combine(base.GetConfigurationBasePath(), "Config", ConfigFolderName);

    protected override IEnumerable<string> GetTestingConfigurationFiles()
        => ["appsettings.json", "appsettings.Testing.json"];
}
