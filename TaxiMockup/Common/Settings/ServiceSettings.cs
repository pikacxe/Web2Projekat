namespace Common.Settings
{
    public class ServiceSettings : ISettingsValidator
    {
        public string? ServiceName { get; init; }

        public bool isValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ServiceName))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
