namespace Common.Settings
{
    public class RideDataServiceSettings : ISettingsValidator
    {
        public string? AppName { get; init; }
        public string? ServiceName { get; init; }
        public string? PartitionKey { get; init; }  
        public string ConnectionString => $"fabric:/{AppName}/{ServiceName}";

        public bool isValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AppName)
                    || string.IsNullOrWhiteSpace(ServiceName)
                    || string.IsNullOrWhiteSpace(PartitionKey))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
