using Demo2.Domain.Events.Repositories;
using Demo2.Domain.Models.EventSourcing;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Demo2.Domain
{
    public static class ChangeRequestGenerator
    {
        private static Random _random = new Random(19001);

        static ChangeRequestGenerator()
        {

        }

        public static async Task<ChangeRequest[]> GenerateAsync(int amount)
        {
            var esResults = new List<ChangeRequest>();
            var sqlResults = new List<ChangeRequest>();

            for (int i = 0; i < amount; i++)
            {
                var amountOfDocuments = _random.Next(1, 10);
                var amountOfFics = _random.Next(1, 10);

                var request = ChangeRequest.CreateByGeneralUser(Generate(4), Generate(4), Generate(50, 500), Generate(25, 100));

                for (int j = 0; j < amountOfDocuments; j++)
                {
                    request.AddFic(Generate(10, 30), Generate(5, 20), Generate(5, 50));
                }

                for (int j = 0; j < amountOfFics; j++)
                {
                    request.AddDocument(Generate(10, 30), Generate(5, 20), Generate(5, 50));
                }

                esResults.Add(request);
            }

            for (int i = 0; i < amount; i++)
            {
                sqlResults.Add(esResults[i]);
            }

            return esResults.ToArray();
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
