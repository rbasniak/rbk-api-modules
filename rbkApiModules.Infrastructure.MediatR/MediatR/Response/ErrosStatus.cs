namespace rbkApiModules.Infrastructure.MediatR.Core
{
    /// <summary>
    /// Possíveis status do comando depois da execução
    /// </summary>
    public enum CommandStatus
    {
        Valid,
        HasHandledError,
        HasUnhandledError
    }
}
