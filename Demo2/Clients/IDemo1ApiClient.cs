namespace Demo2.Clients;

public interface IDemo1ApiClient
{
    Task<string> GetAnonymousMessageAsync(CancellationToken cancellationToken = default);
}
