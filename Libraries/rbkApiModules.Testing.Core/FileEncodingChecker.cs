namespace rbkApiModules.Testing.Core;

public class FileEncodingChecker
{
    public static string[] GetNonUtf8WithBomFiles()
    {
        string[] extensions = [".cs", ".ts", ".css", ".scss", ".html", ".json", ".yml", ".yaml", ".md", ".txt"];

        var repoRoot = GetRepositoryRoot();
        var files = Directory.GetFiles(repoRoot, "*.*", SearchOption.AllDirectories)
            .Where(x => extensions.Contains(Path.GetExtension(x).ToLower()))
            .Where(f => !IsInIgnoredDirectory(f))
            .ToArray();

        var nonCompliantFiles = new List<string>();

        foreach (var file in files)
        {
            if (!IsUtf8WithBom(file))
            {
                nonCompliantFiles.Add(file);
            }
        }

        return nonCompliantFiles.ToArray();
    }

    private static bool IsUtf8WithBom(string filePath)
    {
        var bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var fileBytes = File.ReadAllBytes(filePath);

        if (fileBytes.Length < 3)
        {
            return false;
        }

        return fileBytes.Take(3).SequenceEqual(bom);
    }

    private static string GetRepositoryRoot()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        while (!Directory.Exists(Path.Combine(currentDirectory, ".git")))
        {
            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
            if (currentDirectory == null)
            {
                throw new DirectoryNotFoundException("Cannot find .git folder, make sure you're running the test in a git repository.");
            }
        }

        return currentDirectory;
    }

    private static bool IsInIgnoredDirectory(string filePath)
    {
        string[] ignoredDirectories = ["bin", "obj", "node_modules", ".git", ".code", ".vscode", ".vs", "@TestResults", ".angular", "dist"];

        foreach (var dir in ignoredDirectories)
        {
            if (filePath.Contains(Path.DirectorySeparatorChar + dir + Path.DirectorySeparatorChar))
            {
                return true;
            }
        }

        return false;
    }
}
