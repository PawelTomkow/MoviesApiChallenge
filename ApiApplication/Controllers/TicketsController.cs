using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Tickets;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly IValidator<CreateTicketRequest> _createTicketRequest;
        private readonly IValidator<BuyTicketRequest> _buyTicketRequestValidator;
        private readonly ILogger<TicketsController> _logger;
        private readonly ITicketService _service;

        public TicketsController(IValidator<CreateTicketRequest> createTicketRequest, 
            IValidator<BuyTicketRequest> buyTicketRequestValidator,
            ILogger<TicketsController> logger, 
            ITicketService service)
        {
            _createTicketRequest = createTicketRequest;
            _buyTicketRequestValidator = buyTicketRequestValidator;
            _logger = logger;
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketAsync(string id, CancellationToken cancellationToken)
        {
            var response = await _service.GetAsync(id, cancellationToken);
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTicketRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _createTicketRequest.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                // TODO: Unpack validationResult.Errors to error object response
                return BadRequest(new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request."
                });
            }

            var result = await _service.CreateAsync(request.ReservationId, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyAsync([FromBody] BuyTicketRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _buyTicketRequestValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                // TODO: Unpack validationResult.Errors to error object response
                return BadRequest(new ErrorResponse
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request."
                });
            }

            await _service.BuyAsync(request.TicketId, cancellationToken);
            return Ok();
        }
    }
}