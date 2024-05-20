using Microsoft.ServiceFabric.Services.Remoting;

namespace Contracts
{
    public interface ICDNService : IService 
    {
        /// <summary>
        /// Uploads an image to cdn service
        /// </summary>
        /// <param name="imageBytes"></param>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <returns></returns>
        Task<string> UploadImageAsync(byte[] imageBytes, string fileName, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets a image from cdn service
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <returns></returns>
        Task<byte[]> GetImageAsync(string imageId, CancellationToken cancellationToken = default);
    }
}
