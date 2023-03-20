namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Projectors
{
    //public class ChangeRequestCreatedHandler : INotificationHandler<ChangeRequestCreatedByGeneralUser.V1>
    //{
    //    private readonly IProjectionRepository<ChangeRequest> _projectionRepository;
    //    private readonly IChangeRequestRepository _domainRepository;

    //    //public ChangeRequestCreatedHandler(IProjectionRepository<ChangeRequest> repository, IChangeRequestRepository domainRepository)
    //    //{
    //    //    _projectionRepository = repository;
    //    //    _domainRepository = domainRepository;
    //    //}

    //    public async Task Handle(ChangeRequestCreatedByGeneralUser.V1 notification, CancellationToken cancellation)
    //    {
    //        //var entity = await _domainRepository.FindAsync(notification.AggregateId);

    //        //await _projectionRepository.AddAsync(entity);
    //    }
    //}
}
