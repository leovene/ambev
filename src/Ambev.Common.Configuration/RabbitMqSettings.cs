namespace Ambev.Common.Configuration
{
    public class RabbitMqSettings
    {
        public string Host { get; set; } = string.Empty;
        public string VirtualHost { get; set; } = "/";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}
