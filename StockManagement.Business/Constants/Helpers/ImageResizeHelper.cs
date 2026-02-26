using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace StockManagement.Business.Constants.Helpers
{
    public static class ImageResizeHelper
    {
        public static async Task<MemoryStream> CompressImageAsync(IFormFile file, int targetSizeKB = 130)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentNullException(nameof(file));

            using var inputStream = file.OpenReadStream();
            using var image = await Image.LoadAsync(inputStream);

            long fileSizeKB = file.Length / 1024;
            int maxDimension = fileSizeKB switch
            {
                > 10000 => 800,
                > 5000 => 1024,
                > 2000 => 1280,
                > 1000 => 1600,
                > 500 => 1920,
                _ => 2560
            };

            if (image.Width > maxDimension || image.Height > maxDimension)
            {
                var ratio = Math.Min((double)maxDimension / image.Width, (double)maxDimension / image.Height);
                image.Mutate(x => x.Resize((int)(image.Width * ratio), (int)(image.Height * ratio)));
            }

            var outputStream = new MemoryStream();

            int targetQuality = fileSizeKB switch
            {
                > 10000 => 50,
                > 5000 => 55,
                > 2000 => 65,
                > 1000 => 75,
                _ => 85
            };

            var encoder = GetEncoder(image.Metadata.DecodedImageFormat, targetQuality);
            await image.SaveAsync(outputStream, encoder);

            int attempts = 0;
            int maxAttempts = 20;

            while (outputStream.Length > targetSizeKB * 1024 && targetQuality > 15 && attempts < maxAttempts)
            {
                int qualityStep;
                long currentSizeKB = outputStream.Length / 1024;

                if (currentSizeKB > targetSizeKB * 5)
                    qualityStep = 15;
                else if (currentSizeKB > targetSizeKB * 3)
                    qualityStep = 10;
                else if (currentSizeKB > targetSizeKB * 2)
                    qualityStep = 7;
                else
                    qualityStep = 5;

                targetQuality -= qualityStep;
                if (targetQuality < 15) targetQuality = 15;

                outputStream.SetLength(0);
                outputStream.Position = 0;

                encoder = GetEncoder(image.Metadata.DecodedImageFormat, targetQuality);
                await image.SaveAsync(outputStream, encoder);

                attempts++;
            }

            long finalSizeKB = outputStream.Length / 1024;
            if (finalSizeKB > targetSizeKB)
            {
                Console.WriteLine($"Warning: Could not reach target size. Original: {fileSizeKB}KB, Final: {finalSizeKB}KB, Target: {targetSizeKB}KB, Quality: {targetQuality}, Attempts: {attempts}");
            }
            else
            {
                Console.WriteLine($"Success: Original: {fileSizeKB}KB -> Final: {finalSizeKB}KB (Target: {targetSizeKB}KB) in {attempts} attempts");
            }
            outputStream.Position = 0;
            return outputStream;
        }
        private static IImageEncoder GetEncoder(IImageFormat format, int quality)
        {
            var name = format.Name.ToUpper();
            if (name == "WEBP")
                return new WebpEncoder { Quality = quality };

            return new JpegEncoder { Quality = quality };
        }
    }
}
