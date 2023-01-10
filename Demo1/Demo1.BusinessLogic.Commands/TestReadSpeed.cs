using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Demo1.Database.Read;
using Demo1.Models.Read;
using rbkApiModules.Commons.Core;

namespace Demo1.BusinessLogic.Commands;

public class TestReadSpeed
{
    public class Command : IRequest<CommandResponse>
    {
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
            var result = new Result();

            var sw = Stopwatch.StartNew();

            string message;

            // PERFORMANCE TEST 1
            sw.Restart();
            var entities1a = _context.Set<PerformanceTest1>().AsNoTracking().ToList();
            message = $"PERFORMANCE TEST 1: {entities1a.Count} entities read from database in {sw.ElapsedMilliseconds}ms (with AsNoTracking)";
            Debug.WriteLine(message);
            result.Messages.Add(message);
            sw.Reset();

            _context.ChangeTracker.Clear();

            sw.Start();
            var entities1b = _context.Set<PerformanceTest1>().ToList();
            message = $"PERFORMANCE TEST 1: {entities1b.Count} entities read from database in {sw.ElapsedMilliseconds}ms";
            Debug.WriteLine(message);
            result.Messages.Add(message);

            sw.Reset();

            _context.ChangeTracker.Clear();

            // PERFORMANCE TEST 2
            sw.Start();
            var entities2a = _context.Set<PerformanceTest2>().AsNoTracking().ToList();
            message = $"PERFORMANCE TEST 2: {entities2a.Count} entities read from database in {sw.ElapsedMilliseconds}ms (with AsNoTracking)";
            Debug.WriteLine(message);
            result.Messages.Add(message);
            sw.Reset();

            _context.ChangeTracker.Clear();

            sw.Start();
            var entities2b = _context.Set<PerformanceTest2>().ToList();
            message = $"PERFORMANCE TEST 2: {entities2b.Count} entities read from database in {sw.ElapsedMilliseconds}ms";
            Debug.WriteLine(message);
            result.Messages.Add(message);
            sw.Reset();

            _context.ChangeTracker.Clear();


            // PERFORMANCE TEST 3
            sw.Start();
            var entities3a = _context.Set<PerformanceTest3>().AsNoTracking().ToList();
            message = $"PERFORMANCE TEST 3: {entities3a.Count} entities read from database in {sw.ElapsedMilliseconds}ms (with AsNoTracking)";
            Debug.WriteLine(message);
            result.Messages.Add(message);
            sw.Reset();

            _context.ChangeTracker.Clear();

            sw.Start();
            var entities3b = _context.Set<PerformanceTest3>().ToList();
            message = $"PERFORMANCE TEST 3: {entities3b.Count} entities read from database in {sw.ElapsedMilliseconds}ms";
            Debug.WriteLine(message);
            result.Messages.Add(message);
            sw.Reset();

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