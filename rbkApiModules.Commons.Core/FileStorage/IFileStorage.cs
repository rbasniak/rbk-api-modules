namespace rbkApiModules.Commons.Core;

public interface IFileStorage
{
    /// <summary>
    /// Stores a file from a base64 string and returns the URL
    /// </summary>
    /// <param name="base64FileContent">The base64 encoded file content</param>
    /// <param name="fileName">The file name to use (without extension)</param>
    /// <param name="folderPath">Optional folder path within the storage</param>
    /// <param name="maxWidth">Optional maximum width for image resizing</param>
    /// <param name="maxHeight">Optional maximum height for image resizing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The URL to access the stored file</returns>
    Task<string> StoreFileFromBase64Async(
        string base64FileContent,
        string fileName,
        string? folderPath = null,
        int? maxWidth = null,
        int? maxHeight = null,
        CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Deletes a file from storage
    /// </summary>
    /// <param name="fileUrl">The URL of the file to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
}