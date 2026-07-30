using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using rbkApiModules.Core.Utilities;
using SkiaSharp;

namespace rbkApiModules.Commons.Core;

public class LocalFileStorage : IFileStorage
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalFileStorage> _logger;
    private readonly string _baseUrl;
    private readonly string _uploadDirectory;
    
    public LocalFileStorage(IWebHostEnvironment environment, ILogger<LocalFileStorage> logger)
    {
        _environment = environment;
        _logger = logger;
        
        // Create the uploads directory in the web root if it doesn't exist
        _uploadDirectory = Path.Combine(_environment.WebRootPath, "uploads");
        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
        
        // Base URL for accessing the files - in a real app, this would be configurable
        _baseUrl = "/uploads";
    }
    
    public async Task<string> StoreFileFromBase64Async(
        string base64FileContent,
        string filename,
        string folderPath,
        int? maxWidth = null,
        int? maxHeight = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            folderPath = folderPath.ToLower();

            // Parse the base64 content
            string base64Data = ExtractBase64Data(base64FileContent);
            string fileExtension =ImageUtilities.ExtractExtension(base64FileContent);
            
            // Create the target directory if specified
            string targetDirectory = _uploadDirectory;
            if (!string.IsNullOrEmpty(folderPath))
            {
                targetDirectory = Path.Combine(_uploadDirectory, folderPath);
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
            }
            
            string filePath = Path.Combine(targetDirectory, filename);
            byte[] fileBytes = Convert.FromBase64String(base64Data);
            
            // Process the image if max dimensions are provided and it's an image file
            if ((maxWidth.HasValue || maxHeight.HasValue) && 
                (fileExtension == "jpg" || fileExtension == "jpeg" || fileExtension == "png" || fileExtension == "bmp"))
            {
                await ResizeAndSaveImageAsync(fileBytes, filePath, maxWidth, maxHeight, cancellationToken);
            }
            else
            {
                // Save the file directly
                await File.WriteAllBytesAsync(filePath, fileBytes, cancellationToken);
            }
            
            // Construct the URL
            string relativePath = Path.Combine(folderPath, filename).Replace('\\', '/');
            string url = $"{_baseUrl}/{relativePath}";
            
            _logger.LogInformation("File saved successfully at {FilePath}", filePath);
            
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file from base64 string");
            throw new ApplicationException("Failed to store file", ex);
        }
    }
    
    public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return Task.CompletedTask;
            }
            
            // Extract the relative path from the URL
            if (!fileUrl.StartsWith(_baseUrl))
            {
                _logger.LogWarning("Cannot delete file with URL not matching base URL: {FileUrl}", fileUrl);
                return Task.CompletedTask;
            }
            
            string relativePath = fileUrl.Substring(_baseUrl.Length).TrimStart('/');
            string filePath = Path.Combine(_uploadDirectory, relativePath);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
            }
            else
            {
                _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
            }
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileUrl}", fileUrl);
            throw new ApplicationException($"Failed to delete file: {fileUrl}", ex);
        }
    }
    
    private string ExtractBase64Data(string base64FileContent)
    {
        // Remove data URL prefix if present (e.g., "data:image/jpeg;base64,")
        if (base64FileContent.Contains(","))
        {
            return base64FileContent.Split(',')[1];
        }
        
        return base64FileContent;
    }
    
        
    private async Task ResizeAndSaveImageAsync(
        byte[] imageData, 
        string outputPath, 
        int? maxWidth, 
        int? maxHeight,
        CancellationToken cancellationToken)
    {
        using var inputStream = new MemoryStream(imageData);
        using var bitmap = SKBitmap.Decode(inputStream);
        if (bitmap == null)
        {
            await File.WriteAllBytesAsync(outputPath, imageData, cancellationToken);
            return;
        }

        var targetMaxWidth = maxWidth ?? int.MaxValue;
        var targetMaxHeight = maxHeight ?? int.MaxValue;

        SKBitmap bitmapToEncode = bitmap;
        SKBitmap? resizedBitmap = null;

        if (bitmap.Width > targetMaxWidth || bitmap.Height > targetMaxHeight)
        {
            var ratio = Math.Min(
                (double)targetMaxWidth / bitmap.Width,
                (double)targetMaxHeight / bitmap.Height);
            var newWidth = Math.Max(1, (int)Math.Round(bitmap.Width * ratio));
            var newHeight = Math.Max(1, (int)Math.Round(bitmap.Height * ratio));

            resizedBitmap = bitmap.Resize(new SKImageInfo(newWidth, newHeight), SKSamplingOptions.Default);
            if (resizedBitmap == null)
            {
                await File.WriteAllBytesAsync(outputPath, imageData, cancellationToken);
                return;
            }

            bitmapToEncode = resizedBitmap;
        }

        try
        {
            using var image = SKImage.FromBitmap(bitmapToEncode);
            await SaveEncodedImageAsync(image, outputPath, cancellationToken);
        }
        finally
        {
            resizedBitmap?.Dispose();
        }
    }

    private static async Task SaveEncodedImageAsync(
        SKImage image,
        string outputPath,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(outputPath).TrimStart('.').ToLowerInvariant();
        SKEncodedImageFormat format;
        int quality;

        switch (extension)
        {
            case "png":
                format = SKEncodedImageFormat.Png;
                quality = 100;
                break;
            case "bmp":
                format = SKEncodedImageFormat.Bmp;
                quality = 100;
                break;
            case "jpg":
            case "jpeg":
                format = SKEncodedImageFormat.Jpeg;
                quality = 90;
                break;
            default:
                format = SKEncodedImageFormat.Jpeg;
                quality = 90;
                break;
        }

        using var encoded = image.Encode(format, quality);
        if (encoded == null)
        {
            throw new InvalidOperationException("Failed to encode image.");
        }

        await using var stream = new FileStream(
            outputPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);
        encoded.SaveTo(stream);
        await stream.FlushAsync(cancellationToken);
    }
}