namespace Services.Interface
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Uploads a file to storage and returns its public URL.
        /// </summary>
        Task<string> UploadAsync(Stream content, string fileName, string contentType, string? folder = null);
    }
}
