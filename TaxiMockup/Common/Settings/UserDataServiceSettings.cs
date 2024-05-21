
namespace Common.Settings
{
    public class UserDataServiceSettings : ISettingsValidator
    {
        public string? AppName { get; init; }
        public string? ServiceName { get; init; }
        public string ConnectionString => $"fabric:/{AppName}/{ServiceName}";

        public bool isValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AppName)
                    || string.IsNullOrWhiteSpace(ServiceName))
                {
                    return false;
                }
                return true;
            }
        }

    }
}
