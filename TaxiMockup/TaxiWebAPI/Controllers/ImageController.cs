using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TaxiWebAPI.Settings;

namespace TaxiWebAPI.Controllers
{
    [Route("cdn/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ServiceProxyFactory _proxyFactory;
        private readonly Uri _serviceUri;

        public ImageController(ServiceProxyFactory proxyFactory, CDNServiceSettings serviceSettings)
        {
            _proxyFactory = proxyFactory;
            _serviceUri = new Uri(serviceSettings.ConnectionString);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var fileName = file.FileName;

                var proxy = CreateProxy();
                var imageId = await proxy.UploadImageAsync(imageBytes, fileName);
                var imageUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/cdn/images/{imageId}";
                return Ok(new { image=imageUrl });
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Invalid data");
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid data");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        [Route("{imageId}")]
        public async Task<IActionResult> GetImage(string imageId)
        {
            try
            {
                var proxy = CreateProxy();
                var imageBytes = await proxy.GetImageAsync(imageId);
                return File(imageBytes, "image/png");
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Invalid data");
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid data");
            }
            catch (FileNotFoundException)
            {
                return NotFound("Image not found.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        private ICDNService CreateProxy()
        {
            return _proxyFactory.CreateServiceProxy<ICDNService>(_serviceUri);
        }
    }
}
