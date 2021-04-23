using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace rbkApiModules.VersioningTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

#if DEBUG
            path = @"D:\Repositories\pessoal\libraries\rbk-api-modules-next";
#endif
             
            using (var repo = new Repository(path))
            {
                var lastTag = repo.Tags.OrderByDescending(x => ((Commit)x.Target).Committer.When).First();
                Console.WriteLine($"Last tag: {lastTag.FriendlyName}");
                var allCommits = repo.Commits.OrderByDescending(x => x.Committer.When).ToList();

                var breakingChanges = new List<string>();
                var features = new List<string>();
                var fixes = new List<string>();

                bool hasMajor = false;
                bool hasMinor = false;
                bool hasPatch = false;

                foreach (var commit in allCommits)
                {
                    if (commit.Id == lastTag.Target.Id) break;

                    if (commit.Message.ToLower().StartsWith("fix"))
                    {
                        fixes.Add(commit.Message);
                    }

                    if (commit.Message.ToLower().StartsWith("feat"))
                    {
                        features.Add(commit.Message);
                    }

                    if (commit.Message.ToUpper().StartsWith("BREAKING CHANGE"))
                    {
                        breakingChanges.Add(commit.Message);
                    }
                }

                if (breakingChanges.Count > 0)
                {
                    Console.WriteLine("BREAKING CHANGES: ");
                    foreach (var message in breakingChanges)
                    {
                        Console.WriteLine($"  - {message.Replace("BREAKING CHANGES:", "").Trim()}");
                    }
                    Console.WriteLine(" ");
                }

                if (features.Count > 0)
                {
                    Console.WriteLine("NEW FEATURES: ");
                    foreach (var message in features)
                    {
                        Console.WriteLine($"  - {message.Replace("feat:", "").Trim()}");
                    }
                    Console.WriteLine(" ");
                }

                if (fixes.Count > 0)
                {
                    Console.WriteLine("FIXES: ");
                    foreach (var message in fixes)
                    {
                        Console.WriteLine($"  - {message.Replace("fix:", "").Trim()}");
                    }
                    Console.WriteLine(" ");
                }

                Console.WriteLine("  ");

                var csprojs = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);

                foreach (var file in csprojs)
                {
                    //File.WriteAllText(file, File.ReadAllText(file).Replace("", ""));
                }

                var version = "";
                // repo.Commit(version, new Signature("ci", "ci@github.com", DateTime.UtcNow), new Signature("ci", "ci@github.com", DateTime.UtcNow));

                Console.WriteLine("Done.");
                Console.ReadKey();
            }
        }
    }
}
