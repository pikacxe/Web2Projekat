namespace Common.Settings
{
    public class MongoDBSettings
    {
        public string? Host { get; init; }
        public int Port { get; init; }
        public string? Username { get; init; }
        public string? Password { get; init; }
        //mongodb://<user>:<password>@<host>:<port>/<serviceName>
        public string ConnectionString => $"mongodb://{Username}:{Password}@{Host}:{Port}";
    }
}
