using Microsoft.AspNetCore.Http;

namespace StockManagement.Business.Constants.Helpers
{
    public static class ImageSecurityHelper
    {
        private static readonly Dictionary<string, List<byte[]>> _imageSignatures = new()
        {
            {
                "image/jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },
                    new byte[] { 0xFF, 0xD8, 0xFF }
                }
            },
            {
                "image/jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xDB },
                    new byte[] { 0xFF, 0xD8, 0xFF }
                }
            },
            {
                "image/png", new List<byte[]>
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                }
            },
            {
                "image/webp", new List<byte[]> { }
            }
        };

        public static bool IsValidImageSignature(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            string contentType = file.ContentType.ToLowerInvariant();

            using var stream = file.OpenReadStream();
            using var reader = new BinaryReader(stream);

            byte[] header = reader.ReadBytes(12);
            stream.Position = 0;

            if (_imageSignatures.TryGetValue(contentType, out var signatures) && signatures.Count > 0)
            {
                return signatures.Any(sig => header.Take(sig.Length).SequenceEqual(sig));
            }

            if (contentType == "image/webp" && header.Length >= 12)
            {
                return header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                       header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;
            }

            return false;
        }

        public static bool ContainsMaliciousPatterns(IFormFile file)
        {
            var dangerousPatterns = new[]
            {
                "<?php",
                "<%",
                "<script",
                "eval(",
                "base64_decode",
                "system(",
                "exec(",
                "shell_exec",
                "passthru",
                "<?=",
                "<? "
                //"javascript:"
            };

            if (file == null || file.Length == 0)
                return false;

            if (!file.ContentType?.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ?? true)
                return false;

            const long MaxBytesToCheck = 10 * 1024 * 1024; 

            try
            {
                using var stream = file.OpenReadStream();
                stream.Position = 0;

                int bytesToRead = (int)Math.Min(MaxBytesToCheck, stream.Length);
                byte[] buffer = new byte[bytesToRead];
                int totalRead = 0;

                while (totalRead < bytesToRead)
                {
                    int read = stream.Read(buffer, totalRead, bytesToRead - totalRead);
                    if (read == 0) break;
                    totalRead += read;
                }

                if (totalRead == 0)
                    return false;

                string content = System.Text.Encoding.UTF8.GetString(buffer, 0, totalRead);

                string lowerContent = content.ToLowerInvariant();
                foreach (var pattern in dangerousPatterns)
                {
                    string patternLower = pattern.ToLowerInvariant();
                    if (lowerContent.Contains(patternLower))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidImageContent(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var image = SixLabors.ImageSharp.Image.Load(stream);

                return image.Width > 0 && image.Height > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
