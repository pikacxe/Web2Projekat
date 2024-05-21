using Common.Settings;

namespace TaxiWebAPI.Settings
{
    public class JwtTokenSettings : ISettingsValidator
    {
        public string? Key { get; init; }
        public string? Issuer { get; init; }
        public string? Audience { get; init; }

        public bool isValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Key)
                    || string.IsNullOrWhiteSpace(Issuer)
                    || string.IsNullOrWhiteSpace(Audience))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
