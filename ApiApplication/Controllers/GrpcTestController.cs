using System;
using System.Threading.Tasks;
using ApiApplication.Clients;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("test/grpc")]
    public class GrpcTestController : ControllerBase
    {
        private readonly IApiClient _apiClient;

        public GrpcTestController(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var result = await _apiClient.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("search/{text}")]
        public async Task<IActionResult> SearchAsync(string text)
        {
            var result = await _apiClient.SearchAsync(text);
            return Ok(result);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _apiClient.GetAllAsync();
            return Ok(result);
        }
    }
}