using Demo2.Domain.Events;
using Demo2.Domain.Events.Infrastructure;
using Demo2.Domain.Events.MyImplementation.Database;
using Demo2.Infrastructure.EventSourcing.Database.Repositories;
using Demo2.Relational;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using System.Diagnostics;

namespace Demo2.Domain
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

            var esResults = new List<EventSourcing.ChangeRequest>();
            var rlResults = new List<Relational.ChangeRequest>();

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

            if (rlContext.Set<Relational.Platform>().None())
            {
                un = new Relational.Un("BUZIOS", "UN-BUZIOS", "BUZIOS");

                platforms = new List<Relational.Platform>
                {
                    new Relational.Platform("P-55", un),
                    new Relational.Platform("P-66", un),
                    new Relational.Platform("P-76", un),
                    new Relational.Platform("P-77", un),
                };

                crPriorities = new List<Relational.ChangeRequestPriority>()
                {
                    new Relational.ChangeRequestPriority("Low", ""),
                    new Relational.ChangeRequestPriority("Medium", ""),
                    new Relational.ChangeRequestPriority("High", ""),
                    new Relational.ChangeRequestPriority("Urgent", ""),
                };

                crSources = new List<Relational.ChangeRequestSource>()
                {
                    new Relational.ChangeRequestSource("Source 1"),
                    new Relational.ChangeRequestSource("Source 2"),
                    new Relational.ChangeRequestSource("Source 3"),
                    new Relational.ChangeRequestSource("Source 4"),
                    new Relational.ChangeRequestSource("Source 5"),
                    new Relational.ChangeRequestSource("Source 6"),
                    new Relational.ChangeRequestSource("Source 7"),
                    new Relational.ChangeRequestSource("Source 8"),
                    new Relational.ChangeRequestSource("Source 9"),
                    new Relational.ChangeRequestSource("Source 10"),
                };

                crDisciplines = new List<Relational.Discipline>()
                {
                    new Relational.Discipline("Discipline 1"),
                    new Relational.Discipline("Discipline 2"),
                    new Relational.Discipline("Discipline 3"),
                    new Relational.Discipline("Discipline 4"),
                    new Relational.Discipline("Discipline 5"),
                    new Relational.Discipline("Discipline 6"),
                    new Relational.Discipline("Discipline 7"),
                    new Relational.Discipline("Discipline 8"),
                    new Relational.Discipline("Discipline 9"),
                    new Relational.Discipline("Discipline 10"),
                    new Relational.Discipline("Discipline 11"),
                    new Relational.Discipline("Discipline 12"),
                    new Relational.Discipline("Discipline 13"),
                    new Relational.Discipline("Discipline 14"),
                    new Relational.Discipline("Discipline 15"),
                    new Relational.Discipline("Discipline 16"),
                    new Relational.Discipline("Discipline 17"),
                    new Relational.Discipline("Discipline 18"),
                    new Relational.Discipline("Discipline 19"),
                    new Relational.Discipline("Discipline 20"),
                };

                crTypes = new List<Relational.ChangeRequestType>()
                {
                    new Relational.ChangeRequestType("Type 1"),
                    new Relational.ChangeRequestType("Type 2"),
                    new Relational.ChangeRequestType("Type 3"),
                    new Relational.ChangeRequestType("Type 4"),
                    new Relational.ChangeRequestType("Type 5"),
                };

                crDocCategories = new List<Relational.DocumentCategory>()
                {
                    new Relational.DocumentCategory("Category 1"),
                    new Relational.DocumentCategory("Category 2"),
                    new Relational.DocumentCategory("Category 3"),
                    new Relational.DocumentCategory("Category 4"),
                    new Relational.DocumentCategory("Category 5")
                };

                crFicCategories = new List<Relational.FicCategory>()
                {
                    new Relational.FicCategory("Category 1"),
                    new Relational.FicCategory("Category 2"),
                    new Relational.FicCategory("Category 3"),
                    new Relational.FicCategory("Category 4"),
                    new Relational.FicCategory("Category 5")
                };

                crAttachmentTypes = new List<Relational.AttachmentType>()
                {
                    new Relational.AttachmentType("Type 1", "*.typ1"),
                    new Relational.AttachmentType("Type 2", "*.typ2"),
                    new Relational.AttachmentType("Type 3", "*.typ3"),
                    new Relational.AttachmentType("Type 4", "*.typ4"),
                    new Relational.AttachmentType("Type 5", "*.typ5"),
                };

                crStates = new List<Relational.State>()
                {
                    new Relational.State("State 1"),
                    new Relational.State("State 2"),
                    new Relational.State("State 3"),
                    new Relational.State("State 4"),
                    new Relational.State("State 5"),
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
                var prioritization = new Relational.GutMatrix(_random.Next(0, 4), _random.Next(0, 4), _random.Next(0, 4));
                var sgmStatus = Generate(5, 10);
                var state = GetRandomFromList(crStates);

                var rlRequest = new Relational.ChangeRequest(platform, type, priority, source, state, description,
                    justification, createdBy, requestedBy, currentOwner, sourceNumber, DateTime.UtcNow);

                var esRequest = new EventSourcing.ChangeRequest();

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

                    rlRequest.AddFic(new Relational.Fic(ficName, ficCategory, rlRequest));

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

                    rlRequest.AddDocument(new Relational.Document(docName, docCategory, rlRequest));

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

                    rlRequest.AddAttchment(new Relational.Attachment(attName, attType, attSize, attPath, attFilename, rlRequest));

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

                    rlRequest.AddEvidenceAttchment(new Relational.EvidenceAttachment(attName, attType, attSize, attPath, comment, attFilename, rlRequest));

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

            if (rlContext.Set<Relational.ChangeRequest>().None())
            {
                rlContext.AddRange(rlResults);
                rlContext.SaveChanges();
            }

            var rlSaveElapsed = sw.Elapsed.TotalSeconds;

            sw.Restart();

            await eventStore.SaveAsync(esResults.SelectMany(x => x.UncommittedEvents).Select(x => (IDomainEvent)x));

            var esSaveElapsed = sw.Elapsed.TotalSeconds;

            return new[]
            {
                $"Entities generation time: {generationElapsed:0.00}s",
                $"Entities relational saving time: {rlSaveElapsed:0.00}s",
                $"Entities event sourcing saving time: {esSaveElapsed:0.00}s",
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
