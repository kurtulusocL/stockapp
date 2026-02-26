using AngleSharp.Text;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace StockManagement.Business.Constants.Helpers
{
    public static class ServiceImageHelper
    {
        public static void ImageValidation(IFormFile? image)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Only JPG, PNG, and WEBP files are allowed.");
            }

            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!allowedMimeTypes.Contains(image.ContentType.ToLowerInvariant()))
            {
                throw new ArgumentException("Invalid MIME type.");
            }

            if (!ImageValidationHelper.IsValidImage(image))
            {
                throw new ArgumentException("Geçersiz resim dosyası. Desteklenen formatlar: JPG, JPEG, PNG, WEBP");
            }

            if (!ImageSecurityHelper.IsValidImageSignature(image))
            {
                throw new ArgumentException("File signature does not match image format.");
            }

            //if (ImageSecurityHelper.ContainsMaliciousPatterns(image))
            //{
            //    throw new ArgumentException("File contains suspicious content.");
            //}

            if (!ImageSecurityHelper.IsValidImageContent(image))
            {
                throw new ArgumentException("The file content is not a valid image.");
            }
        }
        public async static Task<string> ProductImageResize(IFormFile? image)
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"Path is preparing: {directoryPath}");
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Guid.NewGuid().ToString() + ".jpg";
            var filePath = Path.Combine(directoryPath, fileName);

            long fileSizeKB = image.Length / 1024;

            bool isNotPng = Path.GetExtension(image.FileName).ToLower() != ".png";
            if (fileSizeKB <= 130 && isNotPng)
            {
                using var originalImage = await Image.LoadAsync(image.OpenReadStream());

                originalImage.Metadata.ExifProfile = null;
                originalImage.Metadata.IccProfile = null;
                originalImage.Metadata.IptcProfile = null;
                originalImage.Metadata.XmpProfile = null;

                var encoder = new JpegEncoder
                {
                    Quality = 85,
                    ColorType = JpegEncodingColor.YCbCrRatio420
                };

                await originalImage.SaveAsJpegAsync(filePath, encoder);
            }
            else
            {
                try
                {
                    using var compressedStream = await ImageResizeHelper.CompressImageAsync(image, 130);
                    using var loadedImage = await Image.LoadAsync(compressedStream);

                    loadedImage.Metadata.ExifProfile = null;
                    loadedImage.Metadata.IccProfile = null;
                    loadedImage.Metadata.IptcProfile = null;
                    loadedImage.Metadata.XmpProfile = null;

                    var encoder = new JpegEncoder
                    {
                        Quality = 85,
                        ColorType = JpegEncodingColor.YCbCrRatio420
                    };

                    await loadedImage.SaveAsJpegAsync(filePath, encoder);
                }
                catch (Exception ex)
                {
                    throw new Exception("An unexpected error occurred while adding the entity.", ex);
                }
            }
            return fileName;
        }
    }
}
