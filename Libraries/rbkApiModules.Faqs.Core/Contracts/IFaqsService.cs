namespace rbkApiModules.Faqs.Core;

public interface IFaqsService
{
    Task<bool> ExistsAsync(string tenant, Guid id, CancellationToken cancellation = default);
    Task<Faq[]> GetAllAsync(string tenant, string tag, CancellationToken cancellation = default);
    Task<Faq> CreateAsync(string tenant, string tag, string question, string answer, CancellationToken cancellation = default);
    Task<Faq> UpdateAsync(string tenant, Guid id, string question, string answer, CancellationToken cancellation = default);
    Task DeleteAsync(string tenant, Guid id, CancellationToken cancellation = default);
}
