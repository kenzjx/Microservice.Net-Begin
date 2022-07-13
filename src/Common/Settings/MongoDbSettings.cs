namespace Common.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; init; }

        public int Port { init; get; }

        public string User { init; get; }

        public string Password { init; get; }

        public string ConnectionString => $"mongodb://{User}:{Password}@{Host}";
    }
}