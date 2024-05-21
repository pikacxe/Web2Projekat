using Common.Settings;

namespace TaxiWebAPI.Settings
{
    public class CorsSettings
    {
        public string PolicyName { get; set; }
        public string AllowedHosts { get; set; }

        private readonly IConfiguration _configuration;

        public CorsSettings(IConfiguration configuration, string policyName) 
        {
            _configuration = configuration;
            PolicyName = policyName;
            AllowedHosts = _configuration.GetSection("AllowedHosts").Value ?? throw new ApplicationException("Cors allowed hosts missing");
        }


    }
}
