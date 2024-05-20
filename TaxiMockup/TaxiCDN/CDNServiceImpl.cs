using Microsoft.ServiceFabric.Services.Remoting;
using Contracts;

namespace TaxiCDN
{
    internal class CDNServiceImpl : ICDNService
    {
        public async Task<string> UploadImageAsync(byte[] imageBytes, string fileName, CancellationToken cancellationToken)
        {
            var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            var filePath = Path.Combine("wwwroot/images", newFileName);

            // TODO possible image resizing

            await File.WriteAllBytesAsync(filePath, imageBytes, cancellationToken);
            return newFileName;
        }

        public async Task<byte[]> GetImageAsync(string imageId, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine("wwwroot/images", imageId);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Image not found.");
            }

            var imageBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            return imageBytes;
        }
    }
}