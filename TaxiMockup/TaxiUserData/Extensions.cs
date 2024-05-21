using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaxiUserData.Helpers;
using TaxiUserData.Settings;

namespace TaxiUserData
{
    internal static class Extensions
    {
        public static IServiceCollection AddMailClient(this IServiceCollection services, IConfiguration configuration)
        {
            var mailSettings = configuration.GetSection(nameof(MailClientSettings)).Get<MailClientSettings>();
            if(mailSettings == null)
            {
                throw new ApplicationException("Mail client settings not set");
            }
            var mailClient = new MailHelper(mailSettings);
            services.AddSingleton(mailClient);
            return services;
        }
    }
}
