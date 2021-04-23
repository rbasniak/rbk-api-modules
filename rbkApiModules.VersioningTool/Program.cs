using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace rbkApiModules.VersioningTool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var path = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).Parent.Parent.Parent.Parent.FullName;

            //#if DEBUG
            //            path = @"D:\Repositories\pessoal\libraries\rbk-api-modules-next";
            //#endif

            using (var repo = new Repository(path))
            {
                var lastTag = repo.Tags.OrderByDescending(x => ((Commit)x.Target).Committer.When).First();
                Console.WriteLine($"Last tag: {lastTag.FriendlyName}");
                Console.WriteLine(" ");
                var allCommits = repo.Commits.OrderByDescending(x => x.Committer.When).ToList();

                var breakingChanges = new List<string>();
                var features = new List<string>();
                var fixes = new List<string>();

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

                var version = lastTag.FriendlyName.Split('.').Select(x => Int32.Parse(x)).ToList();

                if (breakingChanges.Count > 0)
                {
                    version[0] = version[0] + 1;
                    version[1] = 0;
                    version[2] = 0;
                }
                else if (features.Count > 0)
                {
                    version[1] = version[1] + 1;
                    version[2] = 0;
                }
                else
                {
                    version[2] = version[2] + 1;
                }

                var newVersion = $"{version[0]}.{version[1]}.{version[2]}";

                var csprojs = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);

                Console.WriteLine("  Old version: " + lastTag.FriendlyName);
                Console.WriteLine("  New version: " + newVersion);

                foreach (var file in csprojs)
                {
                    File.WriteAllText(file, File.ReadAllText(file).Replace($"<Version>{lastTag.FriendlyName}</Version>", $"<Version>{newVersion}</Version>"));
                }

                Thread.Sleep(500);

                var status = repo.RetrieveStatus();
                var filePaths = status.Modified.Select(mods => mods.FilePath).ToList();
                foreach (var filePath in filePaths)
                {
                    repo.Index.Add(filePath);
                    repo.Index.Write();
                }

                repo.Commit($"New release v{newVersion}", new Signature("ci", "ci@github.com", DateTime.UtcNow), new Signature("ci", "ci@github.com", DateTime.UtcNow));
                repo.ApplyTag(newVersion);

                //var remote = repo.Network.Remotes["origin"];
                //var options = new PushOptions();
                //var credentials = new UsernamePasswordCredentials { Username = username, Password = password };
                //options.Credentials = credentials;
                //var pushRefSpec = @"refs/heads/master";
                //repo.Network.Push(remote, pushRefSpec, options, new Signature(username, email, DateTimeOffset.Now),
                //    "pushed changes");

                Console.WriteLine("  ");

                Console.WriteLine("Done.");
            }
        }
    }
}
