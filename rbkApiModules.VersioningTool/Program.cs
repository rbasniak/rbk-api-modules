using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.VersioningTool
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var repo = new Repository(@"D:\Repositories\pessoal\libraries\rbk-api-modules-next"))
            {
                var lastTag = repo.Tags.OrderByDescending(x => ((Commit)x.Target).Committer.When).First();
                var allCommits = repo.Commits.OrderByDescending(x => x.Committer.When).ToList();

                var messages = new List<string>();
                
                foreach (var commit in allCommits)
                {
                    if (commit.Id == lastTag.Target.Id) break;

                    messages.Add(commit.Message);
                    Console.WriteLine(commit.Message);
                }

                Console.WriteLine("Done.");
                Console.ReadKey();
            }
        }
    }
}
