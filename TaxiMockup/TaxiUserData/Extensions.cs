using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mail;

namespace TaxiUserData
{
    public static class Extensions
    {
        public static IServiceCollection AddMailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(serviceProvider =>
            {
                var serviceSettings = configuration.GetSection(nameof(MailServiceSettings)).Get<MailServiceSettings>();
                if (serviceSettings == null)
                {
                    throw new ArgumentNullException(nameof(serviceSettings));
                }
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = serviceSettings.Host ?? string.Empty;
                smtpClient.Port = serviceSettings.Port;
                smtpClient.Credentials = new NetworkCredential(serviceSettings.Username, serviceSettings.Password);
                MailHelper mailHelper = new MailHelper(smtpClient);
                return mailHelper;
            });
            return services;
        }
    }
}
