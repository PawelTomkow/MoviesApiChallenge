using System;
using System.Net;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Tickets;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly IValidator<BuyTicketRequest> _validator;

        public TicketsController(IValidator<BuyTicketRequest> validator)
        {
            _validator = validator;
        }

        [HttpGet("{id}")]
        public async Task<IActivator> GetTicketAsync([FromQuery] Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyTicketAsync([FromBody] BuyTicketRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                // TODO: Unpack validationResult.Errors to error object response
                return BadRequest(new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    ErrorMessage = "Invalid request."
                });
            }
            
            throw new NotImplementedException();
        }
    }
}