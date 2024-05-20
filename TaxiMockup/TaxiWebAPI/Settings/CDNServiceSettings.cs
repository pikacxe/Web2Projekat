namespace TaxiWebAPI.Settings
{
    public class CDNServiceSettings
    {
        public string? AppName { get; init; }
        public string? ServiceName { get; init; }
        public string ConnectionString => $"fabric:/{AppName}/{ServiceName}";
    }
}
