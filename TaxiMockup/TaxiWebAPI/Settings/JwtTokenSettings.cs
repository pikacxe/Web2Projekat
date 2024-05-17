namespace TaxiWebAPI.Settings
{
    public class JwtTokenSettings
    {
        public string Key { get; init; }
        public string Issuer { get; init; }
        public string Audience { get; init; }
    }
}
