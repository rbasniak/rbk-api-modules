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
                "GET /activities",
                "GET /app",
                "POST /auth/login",
                "POST /auth/refresh-token",
                "GET /categories",
                "PUT /categories",
                "POST /categories",
                "GET /images",
                "PUT /images",
                "POST /images",
                "GET /links",
                "PUT /links",
                "POST /links",
                "GET /magazines",
                "PUT /magazines",
                "POST /magazines",
                "POST /paypal",
                "POST /plans",
                "PUT /plans",
                "GET /plans",
                "GET /sellers",
                "POST /sellers",
                "PUT /sellers",
                "POST /templates",
                "PUT /templates",
                "GET /templates",
                "POST /templates/thumbnail",
                "GET /users/client-quota",
                "GET /users/profile",
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
                for (int i = 0; i < 5; i++)
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
                        path,
                        path,
                        random.NextDouble() > 0.9 ? responses[random.Next(0, responses.Length)] : 200,
                        random.Next(50, 3000)
                    );

                    data.Timestamp = currentData;

                    context.Add(data);
                }

                currentData = currentData.AddHours(1);
            }

            context.SaveChanges();
        }
    }
}
