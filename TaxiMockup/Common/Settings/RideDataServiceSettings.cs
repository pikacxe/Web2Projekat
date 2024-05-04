namespace Common.Settings
{
    public class RideDataServiceSettings
    {
        public string? AppName { get; init; }
        public string? ServiceName { get; init; }
        public string? PartitionKey { get; init; }  
        public string ConnectionString => $"fabric:/{AppName}/{ServiceName}";

    }
}
