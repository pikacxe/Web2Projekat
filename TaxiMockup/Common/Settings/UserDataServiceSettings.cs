
namespace Common.Settings
{
    public class UserDataServiceSettings
    {
        public string? AppName { get; init; }
        public string? ServiceName { get; init; }
        public string ConnectionString => $"fabric:/{AppName}/{ServiceName}";

    }
}
