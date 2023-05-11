using Demo2.Samples.Eventsourcing.EventOrientedChanges.Database.Repositories;
using rbkApiModules.Commons.Core;
using System.Diagnostics;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Projectors;

public interface IChangeRequestDomainModelRepository
{
    Task<ChangeRequestDomainModel> FindAsync(Guid id);
    Task<ChangeRequestDomainModel[]> GetAll();
}

public class ChangeRequestDomainModelRepository : IChangeRequestDomainModelRepository
{
    private readonly ListOfValuesRepository _lovRepository;
    private readonly IChangeRequestRepository _changeRequestRepository;

    public ChangeRequestDomainModelRepository(ListOfValuesRepository lovRepository, IChangeRequestRepository changeRequestRepository)
    {
        _lovRepository = lovRepository;
        _changeRequestRepository = changeRequestRepository;
    }

    public async Task<ChangeRequestDomainModel> FindAsync(Guid id)
    {
        var changeRequestDataModel = await _changeRequestRepository.FindAsync(id);

        return Map(changeRequestDataModel);
    }

    public async Task<ChangeRequestDomainModel[]> GetAll()
    {
        var sw = Stopwatch.StartNew();

        var changeRequestDataModels = await _changeRequestRepository.GetAllAsync();

        Debug.WriteLine($"Database entities loading took {sw.Elapsed.TotalSeconds:0.00}s");

        sw.Restart();

        var results = new List<ChangeRequestDomainModel>();

        foreach (var changeRequestDataModel in changeRequestDataModels)
        {
            var changeRequestDomainModel = Map(changeRequestDataModel);

            results.Add(changeRequestDomainModel);
        }

        Debug.WriteLine($"Entity mapping from database models to domain models took {sw.Elapsed.TotalSeconds:0.00}s");

        return results.ToArray();
    }

    private ChangeRequestDomainModel Map(Models.ChangeRequest changeRequestDataModel)
    {
        return new ChangeRequestDomainModel
        {
            Attachments = changeRequestDataModel.Attachments.Select(x => new Attachment
            {
                Id = x.Id,
                Name = x.Name,
                Filename = x.Filename,
                Path = x.Path,
                Size = x.Size,
                Type = new SimpleNamedEntity(x.TypeId.ToString(), _lovRepository.AttachmentTypes[x.TypeId].Name)
            }),
            CheckedBy = changeRequestDataModel.CheckedBy,
            Comments = changeRequestDataModel.Comments,
            Complexity = changeRequestDataModel.Complexity,
            CreatedBy = changeRequestDataModel.CreatedBy,
            CreationDate = changeRequestDataModel.CreationDate,
            CurrentOwner = changeRequestDataModel.CurrentOwner,
            Description = changeRequestDataModel.Description,
            DesiredDate = changeRequestDataModel.DesiredDate,
            Disciplines = changeRequestDataModel.Disciplines.Select(x => new SimpleNamedEntity
            {
                Id = x.ToString(),
                Name = _lovRepository.Disciplines[x].Name
            }),
            Documents = changeRequestDataModel.Documents.Select(x => new Document
            {
                Id = x.Id,
                Name = x.Name,
                Category = new SimpleNamedEntity(x.CategoryId.ToString(), _lovRepository.DocumentCategories[x.Id].Name)
            }),
            EvidenceAttachments = changeRequestDataModel.EvidenceAttachments.Select(x => new EvidenceAttachment
            {
                Id = x.Id,
                Name = x.Name,
                Filename = x.Filename,
                Path = x.Path,
                Size = x.Size,
                Commentary = x.Commentary,
                AdditionDate = x.AdditionDate,
                Type = new SimpleNamedEntity(x.TypeId.ToString(), _lovRepository.AttachmentTypes[x.TypeId].Name)
            }),
            Fics = changeRequestDataModel.Documents.Select(x => new Fic
            {
                Id = x.Id,
                Name = x.Name,
                Category = new SimpleNamedEntity(x.CategoryId.ToString(), _lovRepository.FicCategories[x.Id].Name)
            }),
            Id = changeRequestDataModel.Id,
            InternalNumber = changeRequestDataModel.InternalNumber,
            Justification = changeRequestDataModel.Justification,
            Platform = new SimpleNamedEntity(changeRequestDataModel.PlatformId.ToString(), _lovRepository.Platforms[changeRequestDataModel.PlatformId].Name),
            Prioritization = new GutMatrix
            {
                Gravity = changeRequestDataModel.Prioritization.Gravity,
                Urgency = changeRequestDataModel.Prioritization.Urgency,
                Tendency = changeRequestDataModel.Prioritization.Tendency,
            },
            Priority = new SimpleNamedEntity(changeRequestDataModel.PriorityId.ToString(), _lovRepository.ChangeRequestPriorities[changeRequestDataModel.PriorityId].Name),
            RequestedBy = changeRequestDataModel.RequestedBy,
            Resource = changeRequestDataModel.Resource,
            Source = new SimpleNamedEntity(changeRequestDataModel.SourceId.ToString(), _lovRepository.ChangeRequestSources[changeRequestDataModel.SourceId].Name),
            SourceNumber = changeRequestDataModel.SourceNumber,
            State = null,
            StatusSgm = changeRequestDataModel.StatusSgm,
            Type = new SimpleNamedEntity(changeRequestDataModel.TypeId.ToString(), _lovRepository.ChangeRequestTypes[changeRequestDataModel.TypeId].Name),
        };
    }
}

public class ChangeRequestDomainModel
{
    required public Guid Id { get; set; }
    required public SimpleNamedEntity Platform { get; set; }
    required public SimpleNamedEntity Type { get; set; }
    required public SimpleNamedEntity Source { get; set; }
    required public string SourceNumber { get; set; }
    required public string RequestedBy { get; set; }
    required public IEnumerable<SimpleNamedEntity> Disciplines { get; set; }
    required public string Description { get; set; }
    required public SimpleNamedEntity Priority { get; set; }
    required public string Justification { get; set; }
    required public SimpleNamedEntity State { get; set; }
    required public string StatusSgm { get; set; }
    required public string Comments { get; set; }
    required public double Complexity { get; set; }
    required public GutMatrix Prioritization { get; set; }
    required public string Resource { get; set; }
    required public string CreatedBy { get; set; }
    required public string CheckedBy { get; set; }
    required public string CurrentOwner { get; set; }
    required public DateTime CreationDate { get; set; }
    required public DateTime? DesiredDate { get; set; }
    required public long InternalNumber { get; set; }
    required public IEnumerable<Fic> Fics { get; set; }
    required public IEnumerable<Document> Documents { get; set; }
    required public IEnumerable<Attachment> Attachments { get; set; }
    required public IEnumerable<EvidenceAttachment> EvidenceAttachments { get; set; }
}

public class Fic
{
    required public Guid Id { get; set; }
    required public string Name { get; set; }
    required public SimpleNamedEntity Category { get; set; }
}

public class GutMatrix
{
    required public int Gravity { get; set; }
    required public int Urgency { get; set; }
    required public int Tendency { get; set; }

    public int Priority => Gravity * Urgency * Tendency;
}

public class EvidenceAttachment
{
    required public Guid Id { get; set; }
    required public string Name { get; set; }
    required public SimpleNamedEntity Type { get; set; }
    required public long Size { get; set; }
    required public DateTime AdditionDate { get; set; }
    required public string Path { get; set; }
    required public string Filename { get; set; }
    required public string Commentary { get; set; }
}

public class Document
{
    required public Guid Id { get; set; }
    required public string Name { get; set; }
    required public SimpleNamedEntity Category { get; set; }
}

public class Attachment
{
    required public Guid Id { get; set; }
    required public string Name { get; set; }
    public SimpleNamedEntity Type { get; set; }
    public long Size { get; set; }
    public string Path { get; set; }
    public string Filename { get; set; }
}