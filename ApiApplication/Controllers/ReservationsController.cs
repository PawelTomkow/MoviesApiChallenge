using System;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts.Reservations;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        public ReservationsController()
        {
            
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationAsync([FromQuery] string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReservationAsync([FromBody] CreateReservationRequest reservationRequest)
        {
            throw new NotImplementedException();
        }
    }
}