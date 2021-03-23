using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace rbkApiModules.Utilities.EFCore
{
    public class DatabaseSeedManager 
    {
        public static bool SeedSuccess = false;
        public static Exception SeedException = null;

        private readonly DbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private Dictionary<string, SeedInfo> _seed;

        public DatabaseSeedManager(DbContext context, IWebHostEnvironment webHostEnvironment, bool automaticMigrations)
        {
            _seed = new Dictionary<string, SeedInfo>();

            _context = context;
            _webHostEnvironment = webHostEnvironment;

            ApplyMigrationsAutomatically = automaticMigrations;
        }

        public bool ApplyMigrationsAutomatically { get; }

        public void AddSeed(string id, SeedInfo data)
        {
            _seed.Add(id, data);
        }

        public void ApplySeed()
        {
            if (ApplyMigrationsAutomatically)
            {
                _context.Database.Migrate();
            }

            SeedSuccess = true;

            foreach (var kvp in _seed)
            {
                if (kvp.Value.UseInProduction || (!kvp.Value.UseInProduction && !_webHostEnvironment.IsProduction()))
                {
                    if (!_context.Set<SeedHistory>().Any(x => x.Id == kvp.Key))
                    {
                        try
                        {
                            using (var transaction = _context.Database.BeginTransaction())
                            {
                                kvp.Value.Function(_context);

                                _context.Add(new SeedHistory(kvp.Key, DateTime.Now));
                                _context.SaveChanges();

                                transaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            SeedSuccess = false;
                            SeedException = ex;
                            break;
                        }
                    }
                }
            }
        }
    }
}