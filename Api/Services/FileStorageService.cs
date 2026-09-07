using Api.Services.Interfaces;

namespace Api.Services
{
    public class FileStorageService : IFileStorageService
    {
        public async Task<string> SaveFileAsync(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.", nameof(file));

            const long maxSize = 10 * 1024 * 1024; // 10 MB
            if (file.Length > maxSize)
                throw new ArgumentException($"File exceeds max size of {maxSize} bytes.", nameof(file));

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".pdf", ".docx", ".txt" };
            if (string.IsNullOrEmpty(extension) || !allowed.Contains(extension))
                throw new ArgumentException($"File extension '{extension}' is not allowed.", nameof(file));

            // Sanitize subFolder to prevent traversal
            if (subFolder.Contains("..") || subFolder.Contains("/") || subFolder.Contains("\\"))
                throw new ArgumentException("Invalid subFolder.", nameof(subFolder));

            var fileName = $"{Guid.NewGuid()}{extension}";
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subFolder);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
