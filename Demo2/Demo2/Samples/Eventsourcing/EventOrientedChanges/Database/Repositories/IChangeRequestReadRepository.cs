using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Database.Repositories
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
