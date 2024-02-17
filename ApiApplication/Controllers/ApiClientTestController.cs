using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Clients;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("test/api-client")]
    public class ApiClientTestController : ControllerBase
    {
        private readonly IApiClient _apiClient;

        public ApiClientTestController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var result = await _apiClient.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("search/{text}")]
        public async Task<IActionResult> SearchAsync(string text, CancellationToken cancellationToken)
        {
            var result = await _apiClient.SearchAsync(text);
            return Ok(result);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await _apiClient.GetAllAsync();
            return Ok(result);
        }
    }
}