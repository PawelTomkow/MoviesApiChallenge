using System;
using System.Net;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Showtimes;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/showtimes")]
    public class ShowtimesController : ControllerBase
    {
        private readonly IValidator<CreateShowtimeRequest> _validator;

        public ShowtimesController(IValidator<CreateShowtimeRequest> validator)
        {
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetShowtimesAsync()
        {
            throw new NotImplementedException();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateShowtimeAsync([FromBody] CreateShowtimeRequest showtimeRequest)
        {
            var validationResult = await _validator.ValidateAsync(showtimeRequest);
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