using System;
using System.Threading.Tasks;
using ApiApplication.Contracts.Showtimes;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/showtimes")]
    public class ShowtimesController : ControllerBase
    {
        public ShowtimesController()
        {
            
        }

        [HttpGet]
        public async Task<IActionResult> GetShowtimesAsync()
        {
            throw new NotImplementedException();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateShowtimeAsync([FromBody] CreateShowtimeRequest showtimeRequest)
        {
            throw new NotImplementedException();
        }
    }
}