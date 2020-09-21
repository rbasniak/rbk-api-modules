using rbkApiModules.Analytics.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace rbkApiModules.Analytics.SqlServer
{
    public static class Seed
    {
        public static void SeedDatabase(SqlServerAnalyticsContext context)
        {
            var random = new Random(32019);

            var users = new[] {
                "designer",
                "Designer",
                "free@gmail.com",
                null,
                "paid@gmail.com"
            };

            var versions = new[] {
                "1.0.1",
                "1.1.5",
                "1.0.0-beta",
            };

            var areas = new[] {
                "links",
                "magazines",
                "sellers",
                "authentication",
            };

            var ips = new[] {
                "::1",
                "139.82.87.23",
                "173.0.82.126",
                "179.109.36.19",
                "179.109.39.199",
                "186.205.89.240",
                "189.122.142.54",
                "50.31.134.17"
            };

            var actions = new[] {
                new SampleRequest("GET /activities",           50, 250, 50),
                new SampleRequest("GET /app",                  60, 600, 75),
                new SampleRequest("POST /auth/login",          55, 50, 150),
                new SampleRequest("POST /auth/refresh-token",  75, 200, 150),
                new SampleRequest("GET /categories",           100, 150, 250),
                new SampleRequest("PUT /categories",           200, 350, 50),
                new SampleRequest("POST /categories",          50, 5000, 25),
                new SampleRequest("GET /images",               50, 200, 75),
                new SampleRequest("PUT /images",               20, 100, 150),
                new SampleRequest("POST /images",              15, 150, 500),
                new SampleRequest("GET /links",                30, 175, 250),
                new SampleRequest("PUT /links",                60, 200, 250),
                new SampleRequest("POST /links",               80, 50, 150),
                new SampleRequest("GET /magazines",            50, 75, 100),
                new SampleRequest("PUT /magazines",            25, 20, 50),
                new SampleRequest("POST /magazines",           75, 10, 25),
                new SampleRequest("POST /paypal",              50, 1000, 25),
                new SampleRequest("POST /plans",               30, 500, 50),
                new SampleRequest("PUT /plans",                75, 100, 100),
                new SampleRequest("GET /plans",                10, 50, 250),
                new SampleRequest("GET /sellers",              5, 75, 250),
                new SampleRequest("POST /sellers",             60, 150, 100),
                new SampleRequest("PUT /sellers",              10, 100, 150),
                new SampleRequest("POST /templates",           20, 50, 150),
                new SampleRequest("PUT /templates",            25, 50, 300),
                new SampleRequest("GET /templates",            10, 50, 3500),
                new SampleRequest("POST /templates/thumbnail", 20, 250, 1500),
                new SampleRequest("GET /users/client-quota",   50, 300, 200),
                new SampleRequest("GET /users/profile",        20, 200, 50),
            };

            var agents = new[] {
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36",
                "PayPal/AUHR-214.0-54656248",
            };

            var responses = new[] {
                400,
                500,
            };

            var startDate = DateTime.Now.AddDays(-30);
            var currentData = startDate;
            
            while (currentData < DateTime.Now)
            {
                var iterations = random.Next(3, 10);

                for (int i = 0; i < iterations; i++)
                {
                    var path = actions[random.Next(0, actions.Length)];

                    var data = new AnalyticsEntry(
                        versions[random.Next(0, versions.Length)],
                        areas[random.Next(0, areas.Length)],
                        "",
                        users[random.Next(0, users.Length)],
                        null,
                        ips[random.Next(0, ips.Length)],
                        agents[random.Next(0, agents.Length)],
                        path.Path,
                        path.Path.Split()[0],
                        path.Path,
                        random.NextDouble() > 0.9 ? responses[random.Next(0, responses.Length)] : 200,
                        path.GetDuration(random),
                        path.GetRequestSize(random),
                        path.GetResponseSize(random)
                    );

                    data.Timestamp = currentData;

                    context.Add(data);
                }

                currentData = currentData.AddHours(1);
            }

            context.SaveChanges();
        }
    }

    public class SampleRequest
    {
        private int _requestSize;
        private int _responseSize;
        private int _duration;

        public SampleRequest(string path, int size1, int size2, int duration)
        {
            Path = path;
            _requestSize = size1;
            _responseSize = size2;
            _duration = duration;
        }

        public string Path { get; set; }
        public int GetRequestSize(Random random)
        {
            return (int)(Math.Sign(random.NextDouble() - 0.5) * (_requestSize * 0.3) + _requestSize);
        }

        public int GetResponseSize(Random random)
        {
            return (int)(Math.Sign(random.NextDouble() - 0.5) * (_responseSize * 0.3) + _responseSize);
        }

        public int GetDuration(Random random)
        {
            return (int)(Math.Sign(random.NextDouble() - 0.5) * (_duration * 0.3) + _duration);
        }
    }
}
