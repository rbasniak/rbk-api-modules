using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace rbkApiModules.VersioningTool
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).Parent.FullName;

#if DEBUG
            path = @"D:\Repositories\pessoal\libraries\rbk-api-modules-next";
#endif

            using (var repo = new Repository(path))
            {
                var allCommits = repo.Commits.OrderByDescending(x => x.Committer.When).ToList();

                var breakingChanges = new List<string>();
                var features = new List<string>();
                var fixes = new List<string>();

                var oldVersion = "1.0.0";

                foreach (var commit in allCommits)
                {
                    if (commit.Message.Contains("Release v"))
                    {
                        oldVersion = commit.Message.Replace("Release v", "");
                        break;
                    }

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

                var version = oldVersion.Split('.').Select(x => Int32.Parse(x)).ToList();

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

                Console.WriteLine("  Old version: " + oldVersion);
                Console.WriteLine("  New version: " + newVersion);

                foreach (var file in csprojs)
                {
                    var contents = File.ReadAllText(file);

                    var regex = new Regex("(?<=<Version>)(.*\n?)(?=</Version>)");

                    contents = regex.Replace(contents, newVersion);

                    File.WriteAllText(file, contents);
                }

                Thread.Sleep(500);

                var status = repo.RetrieveStatus();
                var filePaths = status.Modified.Select(mods => mods.FilePath).ToList();
                foreach (var filePath in filePaths)
                {
                    repo.Index.Add(filePath);
                    repo.Index.Write();
                }

                if (Environment.MachineName != "RB-NOTEBOOK")
                {
                    repo.Commit($"New release v{newVersion}", new Signature("ci", "ci@github.com", DateTime.UtcNow), new Signature("ci", "ci@github.com", DateTime.UtcNow));
                    repo.ApplyTag(newVersion);
                }

                //var remote = repo.Network.Remotes["origin"];
                //var options = new PushOptions();
                //var credentials = new UsernamePasswordCredentials { Username = username, Password = password };
                //options.Credentials = credentials;
                //var pushRefSpec = @"refs/heads/master";
                //repo.Network.Push(remote, pushRefSpec, options, new Signature(username, email, DateTimeOffset.Now), "pushed changes");

                Console.WriteLine("  ");

                Console.WriteLine("Done.");
            }
        }
    }
}
