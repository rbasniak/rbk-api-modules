using Demo2.Domain.Models.EventSourcing;

namespace Demo2.Database.Repositories
{
    public interface IProjectionRepository<T>
    {
        Task AddAsync(T entity);
        Task RemoveAsync(T entity);
        Task ReplaceAsync(T entity);
    }

    public class ChangeRequestReadOnlyRepository : IProjectionRepository<ChangeRequest>
    {
        public async Task AddAsync(ChangeRequest entity)
        {
            
        }

        public async Task RemoveAsync(ChangeRequest entity)
        {
            ;
        }

        public async Task ReplaceAsync(ChangeRequest entity)
        {
            ;
        }
    }


}
