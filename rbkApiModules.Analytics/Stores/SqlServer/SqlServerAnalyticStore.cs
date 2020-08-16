using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics
{
    /// <summary>
    /// Store para SQL Server
    /// </summary>
    public class SqlServerAnalyticStore : IAnalyticStore
    {
        private static readonly IMapper Mapper;
        private readonly string _connectionString;
 
        static SqlServerAnalyticStore()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WebRequest, SqlServerWebRequest>()
                    .ForMember(dest => dest.RemoteIpAddress, x => x.MapFrom(req => req.RemoteIpAddress.ToString()))
                    .ForMember(dest => dest.Id, x => x.Ignore());

                cfg.CreateMap<SqlServerWebRequest, WebRequest>()
                    .ForMember(dest => dest.RemoteIpAddress,
                        x => x.MapFrom(req => IPAddress.Parse(req.RemoteIpAddress)));
            });

            config.AssertConfigurationIsValid();

            Mapper = config.CreateMapper();
        }

        private SqlServerContext GetContext()
        {
            var db = new SqlServerContext(_connectionString);
            db.Database.Migrate();
            return db;
        }

        public SqlServerAnalyticStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task StoreWebRequestAsync(WebRequest request)
        {
            using (var db = GetContext())
            {
                await db.Requests.AddAsync(Mapper.Map<SqlServerWebRequest>(request));
                await db.SaveChangesAsync();
            }
        }

        public Task<IEnumerable<string>> UniqueIdentitiesAsync(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return UniqueIdentitiesAsync(from, to);
        }

        public async Task<IEnumerable<string>> UniqueIdentitiesAsync(DateTime @from, DateTime to)
        {
            using (var db = GetContext())
            {
                return await db.Requests.Where(x => x.Timestamp >= from && x.Timestamp <= to).GroupBy(x => x.Identity)
                    .Select(x => x.Key).ToListAsync();
            }
        }

        public Task<long> CountUniqueIdentitiesAsync(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return CountUniqueIdentitiesAsync(from, to);
        }

        public async Task<long> CountUniqueIdentitiesAsync(DateTime from, DateTime to)
        {
            using (var db = GetContext())
            {
                return await db.Requests.Where(x => x.Timestamp >= from && x.Timestamp <= to).GroupBy(x => x.Identity).CountAsync();
            }
        }

        public async Task<long> CountAsync(DateTime from, DateTime to)
        {
            using (var db = GetContext())
            {
                return await db.Requests.Where(x => x.Timestamp >= from && x.Timestamp <= to).CountAsync();
            }
        }

        public Task<IEnumerable<IPAddress>> IpAddressesAsync(DateTime day)
        {
            var from = day.Date;
            var to = day + TimeSpan.FromDays(1);
            return IpAddressesAsync(from, to);
        }

        public async Task<IEnumerable<IPAddress>> IpAddressesAsync(DateTime from, DateTime to)
        {
            using (var db = GetContext())
            {
                var ips = await db.Requests.Where(x => x.Timestamp >= from && x.Timestamp <= to)
                    .Select(x => x.RemoteIpAddress)
                    .Distinct()
                    .ToListAsync();

                return ips.Select(IPAddress.Parse).ToArray();
            }
        }

        public async Task<IEnumerable<WebRequest>> RequestByIdentityAsync(string identity)
        {
            using (var db = GetContext())
            {
                return await db.Requests.Where(x => x.Identity == identity).Select(x => Mapper.Map<WebRequest>(x)).ToListAsync();
            }
        } 

        public async Task PurgeRequestAsync()
        {
            using (var db = GetContext())
            {
                await db.Database.MigrateAsync();
                db.Requests.RemoveRange(db.Requests);
                await db.SaveChangesAsync();
            }
        } 

        public async Task<IEnumerable<WebRequest>> InTimeRange(DateTime from, DateTime to)
        {
            using (var db = GetContext())
            {
                return (await db.Requests.Where(x => x.Timestamp >= from && x.Timestamp <= to)
                    .ToListAsync())
                    .Select(x => Mapper.Map<WebRequest>(x))
                    .ToList();
            }
        }
    }
}
