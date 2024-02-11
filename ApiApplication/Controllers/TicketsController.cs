using System;
using System.Threading.Tasks;
using ApiApplication.Contracts.Tickets;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        public TicketsController()
        {
            
        }

        [HttpGet("{id}")]
        public async Task<IActivator> GetTicketAsync([FromQuery] Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyTicketAsync([FromBody] BuyTicketRequest request)
        {
            throw new NotImplementedException();
        }
    }
}