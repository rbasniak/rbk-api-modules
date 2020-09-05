namespace rbkApiModules.Infrastructure.MediatR
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
