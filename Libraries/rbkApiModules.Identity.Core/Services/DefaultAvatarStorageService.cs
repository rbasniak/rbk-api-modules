using Microsoft.AspNetCore.Hosting;
using System.Runtime.CompilerServices;

namespace rbkApiModules.Identity.Core;

public class DefaultAvatarStorageService : IAvatarStorage
{
    private readonly IWebHostEnvironment _environment;

    public DefaultAvatarStorageService(IWebHostEnvironment webHostEnvironment)
    {
        _environment = webHostEnvironment;
    }

    public string GetAbsolutePath(string path)
    {
        var wwwrootPath = _environment.WebRootPath.ToLower();

        var pathSeparator = '\\';

        if (wwwrootPath.Contains("/"))
        {
            pathSeparator = '/';
        }

        var parts = path.Split(pathSeparator, StringSplitOptions.RemoveEmptyEntries).ToList();

        parts.Insert(0, wwwrootPath);

        var result = Path.Combine(parts.ToArray());

        return result.ToLower();
    }

    public string GetRelativePath(string filename)
    {
        var wwwrootPath = _environment.WebRootPath.ToLower();

        var result = filename.ToLower().Replace(wwwrootPath, "").Replace("\\", "/");

        return result;
    }

    public async Task<string> SaveAsync(string base64Data, string path, string filenameWithoutExtension, string extension, CancellationToken cancellation)
    {
        var wwwrootPath = _environment.WebRootPath;

        List<string> parts;

        if (path.Contains("/"))
        {
            parts = path.Split('/').ToList();
        }
        else
        {
            parts = path.Split('\\').ToList();
        }

        parts.Insert(0, wwwrootPath);
        parts.Add($"{filenameWithoutExtension}.{extension}".ToLower());

        var outputPath = Path.Combine(parts.ToArray());

        var bytes = Convert.FromBase64String(base64Data.Substring(base64Data.IndexOf(',') + 1));

        await File.WriteAllBytesAsync(outputPath, bytes, cancellation);

        return outputPath.ToLower();
    }

    public async Task<string> SaveAsync(string base64Data, string path, string filenameWithoutExtension, CancellationToken cancellation)
    {
        var mimeType = base64Data.Substring(5, base64Data.IndexOf(";") - 5);

        Dictionary<string, string> extensionMap = new Dictionary<string, string>
        {
            { "image/jpeg", ".jpg" },
            { "image/gif", ".gif" },
            { "image/png", ".png" },
            { "image/bmp", ".bmp" },
            { "image/tiff", ".tif" },
            { "image/svg+xml", ".svg" },
        };

        var extension = "";

        if (extensionMap.ContainsKey(mimeType))
        {
            extension = extensionMap[mimeType];
        }

        return await SaveAsync(base64Data, path, filenameWithoutExtension, extension, cancellation);
    } 
}