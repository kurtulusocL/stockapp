using AngleSharp.Text;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace StockManagement.Business.Constants.Helpers
{
    public static class ImageValidationHelper
    {
        public static bool IsValidImage(IFormFile file, long maxSizeBytes = 10 * 1024 * 1024)
        {
            if (file == null || file.Length == 0 || file.Length > maxSizeBytes)
                return false;

            try
            {
                using var stream = file.OpenReadStream();

                IImageFormat? format = Image.DetectFormat(stream);

                if (format == null)
                    return false;

                var allowedFormats = new IImageFormat[]
                {
                    JpegFormat.Instance,
                    PngFormat.Instance,
                    WebpFormat.Instance
                };

                bool isAllowed = allowedFormats.Any(f =>
                    string.Equals(f.Name, format.Name, StringComparison.OrdinalIgnoreCase));

                if (!isAllowed)
                    return false;

                stream.Position = 0;
                using var testImage = Image.Load(stream);

                return true;
            }
            catch (UnknownImageFormatException)
            {
                return false;
            }
            catch (ImageFormatException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool HasValidImageHeader(IFormFile file)
        {
            if (file == null || file.Length < 12)
                return false;

            try
            {
                using var stream = file.OpenReadStream();
                var buffer = new byte[12];
                int read = stream.Read(buffer, 0, 12);

                if (read < 8) return false;

                // JPEG
                if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                    return true;

                // PNG
                if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                    return true;

                // WEBP
                if (buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46 &&
                    buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }
        public static bool HasAllowedExtension(IFormFile file, string[] allowedExtensions)
        {
            if (file == null) return false;
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(ext);
        }
    }
}
