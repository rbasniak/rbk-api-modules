using Demo2.Database.Repositories;
using Demo2.Domain.Events.Repositories;
using Demo2.EventSourcing;
using MediatR;

namespace Demo2.Domain.Events.ChangeRequests.Projectors
{
    public class ChangeRequestCreatedHandler : INotificationHandler<ChangeRequestCreatedByGeneralUser.V1>
    {
        private readonly IProjectionRepository<ChangeRequest> _projectionRepository;
        private readonly IChangeRequestRepository _domainRepository;

        //public ChangeRequestCreatedHandler(IProjectionRepository<ChangeRequest> repository, IChangeRequestRepository domainRepository)
        //{
        //    _projectionRepository = repository;
        //    _domainRepository = domainRepository;
        //}

        public async Task Handle(ChangeRequestCreatedByGeneralUser.V1 notification, CancellationToken cancellation)
        {
            //var entity = await _domainRepository.FindAsync(notification.AggregateId);

            //await _projectionRepository.AddAsync(entity);
        }
    }
}
