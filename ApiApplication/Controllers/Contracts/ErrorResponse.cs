using System.Net;

namespace ApiApplication.Controllers.Contracts
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}