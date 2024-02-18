using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Reservations;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly IValidator<CreateReservationRequest> _validator;
        private readonly ILogger<ReservationsController> _logger;
        private readonly IReservationService _service;

        public ReservationsController(IValidator<CreateReservationRequest> validator, 
            ILogger<ReservationsController> logger, 
            IReservationService service)
        {
            _validator = validator;
            _logger = logger;
            _service = service;
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservationByIdAsync(string id, CancellationToken cancellationToken)
        {
            var result = await _service.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReservationAsync([FromBody] CreateReservationRequest reservationRequest, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(reservationRequest, cancellationToken);
            if (!validationResult.IsValid)
            {
                // TODO: Unpack validationResult.Errors to error object response
                return BadRequest(new ErrorResponse()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request."
                });
            }

            var result = await _service.CreateAsync(reservationRequest.AuditoriumId, reservationRequest.ShowtimeId,
                reservationRequest.Seats, cancellationToken);
            return StatusCode((int)HttpStatusCode.Created, result);
        }
    }
}