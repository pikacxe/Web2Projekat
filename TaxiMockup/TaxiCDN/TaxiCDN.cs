using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace TaxiCDN
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class TaxiCDN : StatelessService
    {
        public TaxiCDN(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var builder = WebApplication.CreateBuilder();

                        builder.Services.AddSingleton<StatelessServiceContext>(serviceContext);
                        builder.WebHost
                                    .UseKestrel()
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url);
                        var app = builder.Build();

                        // Configure route to accept image upload
                        app.MapPost("/cdn/upload", UploadImage);

                        // Configure route to retrieve image by URL
                        app.MapGet("/cdn/image/{fileName}", GetImage);

                        return app;

                    }))
            };
        }

        private async Task UploadImage(HttpContext context)
        {
            var files = context.Request.Form.Files;
            if (files.Count == 0)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("No file uploaded.");
                return;
            }

            var file = files[0];
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine("wwwroot/images", fileName);

            // TODO possible image resizing

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var imageUrl = $"{context.Request.Scheme}://{context.Request.Host}/cdn/image/{fileName}";
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync(imageUrl);
        }

        private async Task GetImage(HttpContext context)
        {
            var fileName = context.Request.RouteValues["fileName"].ToString();
            if(string.IsNullOrWhiteSpace(fileName))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Image not found.");
                return;
            }
            var filePath = Path.Combine("wwwroot/images", fileName);

            if (!File.Exists(filePath))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Image not found.");
                return;
            }

            context.Response.Headers.Append("Content-Type", "image/png");
            await context.Response.SendFileAsync(filePath);
        }
    }
}
