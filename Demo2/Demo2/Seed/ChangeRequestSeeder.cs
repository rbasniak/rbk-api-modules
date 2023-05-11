using Demo2.Samples.Eventsourcing.EventOrientedChanges.Database;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Commands.Attachments;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Commands.ChangeRequests;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Commands.Documents;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Commands.EvidenceAttachments;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Commands.Fics;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Database.Repositories;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;
using Demo2.Samples.Relational.Database;
using Demo2.Samples.Relational.Database.Config.Relational;
using Demo2.Samples.Relational.Domain.Models;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core;
using System.Diagnostics;

namespace Demo2.Seed
{
    public static class ChangeRequestSeeder
    {
        private static Random _random = new Random(19001);

        static ChangeRequestSeeder()
        {

        }

        public async static Task<string[]> Generate(RelationalContext rlContext, IEventStore eventStore, int amount)
        {
            var sw = Stopwatch.StartNew();

            var esResults = new List<Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models.ChangeRequest>();
            var rlResults = new List<Demo2.Samples.Relational.Domain.Models.ChangeRequest>();

            var un = rlContext.Set<Un>().FirstOrDefault();
            var platforms = rlContext.Set<Platform>().ToList();
            var crPriorities = rlContext.Set<ChangeRequestPriority>().ToList();
            var crSources = rlContext.Set<ChangeRequestSource>().ToList();
            var crDisciplines = rlContext.Set<Discipline>().ToList();
            var crTypes = rlContext.Set<ChangeRequestType>().ToList();
            var crDocCategories = rlContext.Set<DocumentCategory>().ToList();
            var crFicCategories = rlContext.Set<FicCategory>().ToList();
            var crAttachmentTypes = rlContext.Set<AttachmentType>().ToList();
            var crStates = rlContext.Set<State>().ToList();

            if (rlContext.Set<Platform>().None())
            {
                un = new Un("BUZIOS", "UN-BUZIOS", "BUZIOS");

                platforms = new List<Platform>
                {
                    new Platform("P-55", un),
                    new Platform("P-66", un),
                    new Platform("P-76", un),
                    new Platform("P-77", un),
                };

                crPriorities = new List<ChangeRequestPriority>()
                {
                    new ChangeRequestPriority("Low", ""),
                    new ChangeRequestPriority("Medium", ""),
                    new ChangeRequestPriority("High", ""),
                    new ChangeRequestPriority("Urgent", ""),
                };

                crSources = new List<ChangeRequestSource>()
                {
                    new ChangeRequestSource("Source 1"),
                    new ChangeRequestSource("Source 2"),
                    new ChangeRequestSource("Source 3"),
                    new ChangeRequestSource("Source 4"),
                    new ChangeRequestSource("Source 5"),
                    new ChangeRequestSource("Source 6"),
                    new ChangeRequestSource("Source 7"),
                    new ChangeRequestSource("Source 8"),
                    new ChangeRequestSource("Source 9"),
                    new ChangeRequestSource("Source 10"),
                };

                crDisciplines = new List<Discipline>()
                {
                    new Discipline("Discipline 1"),
                    new Discipline("Discipline 2"),
                    new Discipline("Discipline 3"),
                    new Discipline("Discipline 4"),
                    new Discipline("Discipline 5"),
                    new Discipline("Discipline 6"),
                    new Discipline("Discipline 7"),
                    new Discipline("Discipline 8"),
                    new Discipline("Discipline 9"),
                    new Discipline("Discipline 10"),
                    new Discipline("Discipline 11"),
                    new Discipline("Discipline 12"),
                    new Discipline("Discipline 13"),
                    new Discipline("Discipline 14"),
                    new Discipline("Discipline 15"),
                    new Discipline("Discipline 16"),
                    new Discipline("Discipline 17"),
                    new Discipline("Discipline 18"),
                    new Discipline("Discipline 19"),
                    new Discipline("Discipline 20"),
                };

                crTypes = new List<ChangeRequestType>()
                {
                    new ChangeRequestType("Type 1"),
                    new ChangeRequestType("Type 2"),
                    new ChangeRequestType("Type 3"),
                    new ChangeRequestType("Type 4"),
                    new ChangeRequestType("Type 5"),
                };

                crDocCategories = new List<DocumentCategory>()
                {
                    new DocumentCategory("Category 1"),
                    new DocumentCategory("Category 2"),
                    new DocumentCategory("Category 3"),
                    new DocumentCategory("Category 4"),
                    new DocumentCategory("Category 5")
                };

                crFicCategories = new List<FicCategory>()
                {
                    new FicCategory("Category 1"),
                    new FicCategory("Category 2"),
                    new FicCategory("Category 3"),
                    new FicCategory("Category 4"),
                    new FicCategory("Category 5")
                };

                crAttachmentTypes = new List<AttachmentType>()
                {
                    new AttachmentType("Type 1", "*.typ1"),
                    new AttachmentType("Type 2", "*.typ2"),
                    new AttachmentType("Type 3", "*.typ3"),
                    new AttachmentType("Type 4", "*.typ4"),
                    new AttachmentType("Type 5", "*.typ5"),
                };

                crStates = new List<State>()
                {
                    new State("State 1"),
                    new State("State 2"),
                    new State("State 3"),
                    new State("State 4"),
                    new State("State 5"),
                };

                rlContext.AddRange(platforms);
                rlContext.AddRange(crPriorities);
                rlContext.AddRange(crSources);
                rlContext.AddRange(crDisciplines);
                rlContext.AddRange(crTypes);
                rlContext.AddRange(crDocCategories);
                rlContext.AddRange(crFicCategories);
                rlContext.AddRange(crAttachmentTypes);
                rlContext.AddRange(crStates);

                rlContext.SaveChanges();
            }

            for (int i = 0; i < amount; i++)
            {
                var amountOfDocuments = _random.Next(1, 10);
                var amountOfFics = _random.Next(1, 10);
                var amountOfAttachments = _random.Next(1, 10);
                var amountOfEvidenceAttachments = _random.Next(1, 5);
                var amountOfDisciplines = _random.Next(0, 3);

                var platform = GetRandomFromList(platforms);
                var priority = GetRandomFromList(crPriorities);
                var source = GetRandomFromList(crSources);
                var type = GetRandomFromList(crTypes);
                var description = Generate(25, 500);
                var justification = Generate(0, 100);
                var sourceNumber = Generate(5, 30);
                var createdBy = Generate(4);
                var requestedBy = Generate(4);
                var currentOwner = Generate(4);
                var comments = Generate(0, 500);
                var complexity = _random.Next(1, 10);
                var prioritization = new GutMatrix(_random.Next(0, 4), _random.Next(0, 4), _random.Next(0, 4));
                var sgmStatus = Generate(5, 10);
                var state = GetRandomFromList(crStates);

                var rlRequest = new ChangeRequest(platform, type, priority, source, state, description,
                    justification, createdBy, requestedBy, currentOwner, sourceNumber, DateTime.UtcNow);

                var esRequest = new Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models.ChangeRequest();

                var createCommand = new CreateChangeRequestByGeneralUser.Request
                {
                    Description = description,
                    Justification = justification,
                    PlatformId = platform.Id,
                    PriorityId = priority.Id,
                    RequestedBy = requestedBy,
                    SourceId = source.Id,
                    SourceNumber = sourceNumber,
                    TypeId = type.Id,
                };
                esRequest.Execute(createCommand);

                for (int j = 0; j < amountOfDocuments; j++)
                {
                    var ficName = Generate(5, 30);
                    var ficCategory = GetRandomFromList(crFicCategories);

                    rlRequest.AddFic(new Fic(ficName, ficCategory, rlRequest));

                    var addFicCommand = new AddFicToChangeRequest.Request
                    {
                        CategoryId = ficCategory.Id,
                        ChangeRequestId = createCommand.ChangeRequestId,
                        Name = ficName,
                    };
                    esRequest.Execute(addFicCommand);
                }

                for (int j = 0; j < amountOfFics; j++)
                {
                    var docName = Generate(5, 30);
                    var docCategory = GetRandomFromList(crDocCategories);

                    rlRequest.AddDocument(new Document(docName, docCategory, rlRequest));

                    var addDocCommand = new AddDocumentToChangeRequest.Request
                    {
                        CategoryId = docCategory.Id,
                        ChangeRequestId = createCommand.ChangeRequestId,
                        Name = docName,
                    };
                    esRequest.Execute(addDocCommand);
                }

                for (int j = 0; j < amountOfAttachments; j++)
                {
                    var attName = Generate(5, 30);
                    var attType = GetRandomFromList(crAttachmentTypes);
                    var attSize = _random.Next(1000, 100000);
                    var attPath = Generate(25, 255);
                    var attFilename = Generate(5, 15);

                    rlRequest.AddAttchment(new Attachment(attName, attType, attSize, attPath, attFilename, rlRequest));

                    var addAttachmentCommand = new AddAttachmentToChangeRequest.Request
                    {
                        ChangeRequestId = createCommand.ChangeRequestId,
                        Filename = attFilename,
                        Name = attName,
                        Path = attPath,
                        Size = attSize,
                        TypeId = attType.Id,
                        Username = Generate(5, 10),
                    };
                    esRequest.Execute(addAttachmentCommand);
                }

                for (int j = 0; j < amountOfEvidenceAttachments; j++)
                {
                    var attName = Generate(5, 30);
                    var attType = GetRandomFromList(crAttachmentTypes);
                    var attSize = _random.Next(1000, 100000);
                    var attPath = Generate(25, 255);
                    var attFilename = Generate(5, 15);
                    var comment = Generate(100, 1000);

                    rlRequest.AddEvidenceAttchment(new EvidenceAttachment(attName, attType, attSize, attPath, comment, attFilename, rlRequest));

                    var addEvidenceAttachmentCommand = new AddEvidenceAttachmentToChangeRequest.Request
                    {
                        ChangeRequestId = createCommand.ChangeRequestId,
                        Filename = attFilename,
                        Name = attName,
                        Path = attPath,
                        Size = attSize,
                        TypeId = attType.Id,
                        Username = Generate(5, 10),
                        Comments = comment
                    };
                    esRequest.Execute(addEvidenceAttachmentCommand);
                }

                var pickedDisciplines = new List<Discipline>();
                for (int j = 0; j < amountOfDisciplines; j++)
                {
                    var discipline = GetRandomFromList(crDisciplines);
                    rlRequest.AddDiscipline(discipline);
                    pickedDisciplines.Add(discipline);
                }

                var addDisciplinesCommand = new UpdateDisciplines.Request
                {
                    ChangeRequestId = createCommand.ChangeRequestId,
                    DisciplineIds = pickedDisciplines.Select(x => x.Id).ToArray(),
                };
                esRequest.Execute(addDisciplinesCommand);

                rlRequest.SetComplexity(complexity);

                var setComplexityCommand = new UpdateChangeRequestComplexity.Request
                {
                    ChangeRequestId = createCommand.ChangeRequestId,
                    Complexity = complexity,
                };
                esRequest.Execute(setComplexityCommand);

                rlRequest.SetPrioritization(prioritization);

                var setPrioritizationCommand = new UpdateChangeRequestPrioritization.Request
                {
                    ChangeRequestId = createCommand.ChangeRequestId,
                    G = prioritization.Gravity,
                    U = prioritization.Urgency,
                    T = prioritization.Tendency,
                };
                esRequest.Execute(setPrioritizationCommand);

                rlRequest.SetComments(comments);

                var setCommentsCommand = new UpdateChangeRequestComments.Request
                {
                    ChangeRequestId = createCommand.ChangeRequestId,
                    Comments = comments,
                };
                esRequest.Execute(setCommentsCommand);

                rlRequest.SetSgmStatus(sgmStatus);

                var setSgmStatusCommand = new UpdateSgmStatusComments.Request
                {
                    ChangeRequestId = createCommand.ChangeRequestId,
                    SgmStatus = sgmStatus,
                };
                esRequest.Execute(setSgmStatusCommand);

                rlResults.Add(rlRequest);
                esResults.Add(esRequest);
            }

            var generationElapsed = sw.Elapsed.TotalSeconds;
            sw.Restart();

            if (rlContext.Set<ChangeRequest>().None())
            {
                rlContext.AddRange(rlResults);
                rlContext.SaveChanges();
            }

            var rlSaveElapsed = sw.Elapsed.TotalSeconds;

            sw.Restart();

            await eventStore.SaveAsync(esResults.SelectMany(x => x.UncommittedEvents).Select(x => (IDomainEvent)x));

            var esSaveElapsed = sw.Elapsed.TotalSeconds;

            var totalRelational = rlContext.Set<Attachment>().Count() +
                rlContext.Set<AttachmentType>().Count() +
                rlContext.Set<ChangeRequest>().Count() +
                rlContext.Set<ChangeRequestPriority>().Count() +
                rlContext.Set<ChangeRequestSource>().Count() +
                rlContext.Set<ChangeRequestToDiscipline>().Count() +
                rlContext.Set<ChangeRequestType>().Count() +
                rlContext.Set<Discipline>().Count() +
                rlContext.Set<DocumentCategory>().Count() +
                rlContext.Set<Document>().Count() +
                rlContext.Set<EvidenceAttachment>().Count() +
                rlContext.Set<FicCategory>().Count() +
                rlContext.Set<Platform>().Count() +
                rlContext.Set<State>().Count() +
                rlContext.Set<Un>().Count();

            var totalEventSourcing = await eventStore.Count();

            return new[]
            {
                $"Entities generation time: {generationElapsed:0.00}s",
                $"Entities relational saving time: {rlSaveElapsed:0.00}s ({totalRelational} total entities)",
                $"Entities event sourcing saving time: {esSaveElapsed:0.00}s ({totalEventSourcing} events)",
            };
        }

        private static T GetRandomFromList<T>(List<T> items)
        {
            var index = _random.Next(0, items.Count);

            return items[index];
        }

        private static string Generate(int minSize, int maxSize)
        {
            var size = _random.Next(minSize, maxSize);

            var buffer = new byte[size];

            _random.NextBytes(buffer);

            var result = Convert.ToBase64String(buffer);

            return result.Substring(0, size).Replace("+", "0").Replace("/", "1");
        }

        private static string Generate(int size)
        {
            return Generate(size, size);
        }
    }
}
