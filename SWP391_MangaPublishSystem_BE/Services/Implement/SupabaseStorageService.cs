using System.Net.Http.Headers;
using Services.Interface;
using Services.Settings;

namespace Services.Implement
{
    public class SupabaseStorageService : IFileStorageService
    {
        private readonly HttpClient _httpClient;
        private readonly SupabaseSettings _settings;

        public SupabaseStorageService(HttpClient httpClient, SupabaseSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
        }

        public async Task<string> UploadAsync(Stream content, string fileName, string contentType, string? folder = null)
        {
            var baseUrl = _settings.Url.TrimEnd('/');
            var extension = Path.GetExtension(fileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var sanitized = System.Text.RegularExpressions.Regex.Replace(nameWithoutExtension, @"[^a-zA-Z0-9_-]", "_");
            var safeName = Uri.EscapeDataString(sanitized + extension);

            // Unique object path so uploads never overwrite each other.
            var objectPath = string.IsNullOrWhiteSpace(folder)
                ? $"{Guid.NewGuid():N}_{safeName}"
                : $"{folder.Trim('/')}/{Guid.NewGuid():N}_{safeName}";

            var uploadUrl = $"{baseUrl}/storage/v1/object/{_settings.Bucket}/{objectPath}";

            using var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
            request.Headers.TryAddWithoutValidation("apikey", _settings.AnonKey);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AnonKey);
            request.Headers.TryAddWithoutValidation("x-upsert", "true");

            var streamContent = new StreamContent(content);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);
            request.Content = streamContent;

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"Supabase upload failed ({(int)response.StatusCode}): {error}");
            }

            // Public URL (bucket must be public, or front it with a signed URL otherwise).
            return $"{baseUrl}/storage/v1/object/public/{_settings.Bucket}/{objectPath}";
        }
    }
}
