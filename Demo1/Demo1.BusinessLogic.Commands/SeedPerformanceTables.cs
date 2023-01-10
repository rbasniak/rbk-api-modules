using Bogus;
using MediatR;
using System.Diagnostics;
using Demo1.Database.Read;
using Demo1.Models.Read;
using rbkApiModules.Commons.Core;

namespace Demo1.BusinessLogic.Commands;

public class SeedPerformanceTables
{
    public class Command : IRequest<CommandResponse>
    {
        public int Size { get; set; }
    }

    public class Handler : RequestHandler<Command, CommandResponse>
    {
        private readonly ReadDatabaseContext _context;

        public Handler(ReadDatabaseContext context)
        {
            _context = context;
        }

        protected override CommandResponse Handle(Command command)
        {
            var random = new Random();

            var sw = Stopwatch.StartNew();

            var result = new Result();

            int size = command.Size;

            var entities1 = new PerformanceTest1[size];
            var entities2 = new PerformanceTest2[size];
            var entities3 = new PerformanceTest3[size];

            for (int i = 0; i < size; i++)
            {
                var blogId = Guid.NewGuid();
                var authorId = Guid.NewGuid();

                var postFaker = new Faker<PerformanceTest1>()
                    .StrictMode(true)
                    .RuleFor(o => o.AuthorId, f => Guid.NewGuid())
                    .RuleFor(o => o.BlogId, f => Guid.NewGuid())
                    .RuleFor(o => o.Id, f => Guid.NewGuid())
                    .RuleFor(o => o.Author, f => f.Person.FullName)
                    .RuleFor(o => o.Blog, f => f.Lorem.Sentence())
                    .RuleFor(o => o.Title, f => f.Lorem.Sentence(15, 10))
                    .RuleFor(o => o.Body, f => f.Lorem.Paragraphs(random.Next(5, 15)));

                var post = postFaker.Generate();

                entities1[i] = new PerformanceTest1
                {
                    Id = post.Id,
                    AuthorId = post.AuthorId,
                    Author = post.Author,
                    BlogId = post.BlogId,
                    Blog = post.Blog,
                    Body = post.Body,
                    Title = post.Title
                };

                entities2[i] = new PerformanceTest2
                {
                    Id = post.Id,
                    Author = new SimpleNamedEntity(post.AuthorId.Value, post.Author),
                    Blog = new SimpleNamedEntity(post.BlogId.Value, post.Blog),
                    Body = post.Body,
                    Title = post.Title
                };

                entities3[i] = new PerformanceTest3
                {
                    Id = post.Id,
                    Data = new PerformanceTest3.DataModel
                    {
                        Author = new SimpleNamedEntity(post.AuthorId.Value, post.Author),
                        Blog = new SimpleNamedEntity(post.BlogId.Value, post.Blog),
                        Body = post.Body,
                        Title = post.Title
                    }
                };
            }

            var message = $"Entities creation took {sw.ElapsedMilliseconds}ms";
            Debug.WriteLine(message);
            result.Messages.Add(message);


            // PERFORMANCE TEST 1
            sw.Restart();

            _context.Set<PerformanceTest1>().AddRange(entities1);

            message = $"PERFORMANCE TEST 1: entities added to context in {sw.ElapsedMilliseconds}ms";
            Debug.WriteLine(message);
            result.Messages.Add(message);

            sw.Restart();

            _context.SaveChanges();

            message = $"PERFORMANCE TEST 1: entities saved to context in {sw.ElapsedMilliseconds}ms";
            Debug.WriteLine(message);
            result.Messages.Add(message);

            _context.ChangeTracker.Clear();

            // PERFORMANCE TEST 2
            sw.Restart();

            _context.Set<PerformanceTest2>().AddRange(entities2);

            message = $"PERFORMANCE TEST 2: entities added to context in {sw.ElapsedMilliseconds}ms";
            Debug.WriteLine(message);
            result.Messages.Add(message);

            sw.Restart();

            _context.SaveChanges();

            message = $"PERFORMANCE TEST 2: entities saved to context in {sw.ElapsedMilliseconds}ms";
            Debug.WriteLine(message);
            result.Messages.Add(message);


            _context.ChangeTracker.Clear();

            // PERFORMANCE TEST 3
            sw.Restart();

            _context.Set<PerformanceTest3>().AddRange(entities3);

            message = $"PERFORMANCE TEST 3: entities added to context in {sw.ElapsedMilliseconds}ms";
            Debug.WriteLine(message);
            result.Messages.Add(message);

            sw.Restart();

            _context.SaveChanges();

            message = $"PERFORMANCE TEST 3: entities saved to context in {sw.ElapsedMilliseconds}ms";
            Debug.WriteLine(message);
            result.Messages.Add(message);

            _context.ChangeTracker.Clear();

            return CommandResponse.Success(result);
        } 
    }

    public class Result
    {
        public Result()
        {
            Messages = new List<string>();
        }

        public List<string> Messages { get; set; }
    }
}