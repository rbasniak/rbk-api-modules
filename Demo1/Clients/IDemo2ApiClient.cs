namespace Demo1.Clients;

public interface IDemo2ApiClient
{
    Task<string> GetAnonymousMessageAsync(CancellationToken cancellationToken = default);
}
