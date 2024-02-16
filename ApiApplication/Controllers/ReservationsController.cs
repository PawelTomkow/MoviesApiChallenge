using System;
using System.Net;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Reservations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly IValidator<CreateReservationRequest> _validator;

        public ReservationsController(IValidator<CreateReservationRequest> validator)
        {
            _validator = validator;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReservationAsync([FromBody] CreateReservationRequest reservationRequest)
        {
            var validationResult = await _validator.ValidateAsync(reservationRequest);
            if (!validationResult.IsValid)
            {
                // TODO: Unpack validationResult.Errors to error object response
                return BadRequest(new ErrorResponse()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request."
                });
            }
            
            throw new NotImplementedException();
        }
    }
}