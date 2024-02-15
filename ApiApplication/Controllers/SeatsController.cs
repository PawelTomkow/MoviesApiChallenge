using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/seats")]
    public class SeatsController : ControllerBase
    {
        public SeatsController()
        {
            
        }

        [HttpGet("{auditoriumId}")]
        public async Task<IActionResult> GetSeatsByAuditoriumIdAsync(int auditoriumId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("status/{showtimeId}")]
        public async Task<IActionResult> GetShowtimeSeatsWithStatusAsync(int showtimeId)
        {
            throw new NotImplementedException();
        }
    }
}