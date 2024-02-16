using ApiApplication.Core.Exceptions;

namespace ApiApplication.Configurations
{
    public class ApiClientConfiguration
    {
        public const string Name = "ApiClient";
        
        public string BaseAddress { get; set; }
        public string ApiKey { get; set; }
        public string Type { get; set; }
    }
}