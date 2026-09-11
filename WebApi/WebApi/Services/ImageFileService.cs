using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace WebApi.Services
{
    public class ImageFileService(IWebHostEnvironment environment)
    {
        private const long MaxImageSize = 5 * 1024 * 1024;
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp"
        };

        private readonly IWebHostEnvironment _environment = environment;

        public string? ValidateImage(IFormFile? imageFile)
        {
            if (imageFile is null)
                return null;

            var extension = Path.GetExtension(imageFile.FileName);
            if (!AllowedImageExtensions.Contains(extension))
                return "Only JPG, JPEG, PNG, GIF, and WEBP images are allowed.";

            if (!imageFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return "The uploaded file must be an image.";

            if (imageFile.Length == 0 || imageFile.Length > MaxImageSize)
                return "The image must be between 1 byte and 5 MB.";

            return null;
        }

        public async Task<string?> SaveImageAsync(IFormFile? imageFile, HttpRequest request, string imageFolder)
        {
            if (imageFile is null)
                return null;

            var uploadDirectory = GetUploadDirectory(imageFolder);
            Directory.CreateDirectory(uploadDirectory);

            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var fileName = $"{Convert.ToHexString(RandomNumberGenerator.GetBytes(16))}{extension}";
            var filePath = Path.Combine(uploadDirectory, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await imageFile.CopyToAsync(stream);

            return $"{request.Scheme}://{request.Host}/uploads/{imageFolder}/{fileName}";
        }

        public void DeleteImage(string? imageUrl, string imageFolder)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            var fileName = Path.GetFileName(imageUrl);
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var filePath = Path.Combine(GetUploadDirectory(imageFolder), fileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        private string GetUploadDirectory(string imageFolder)
        {
            var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            return Path.Combine(webRootPath, "uploads", imageFolder);
        }
    }
}
