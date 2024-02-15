namespace ApiApplication.Configurations
{
    public class ApiClientConfiguration
    {
        public const string ApiClient = "ApiClient";
        
        public string BaseAddress { get; set; }
        public string ApiKey { get; set; }
        public string Type { get; set; }
    }
}