using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Showtimes;
using ApiApplication.Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/showtimes")]
    public class ShowtimesController : ControllerBase
    {
        private readonly IShowtimeService _service;
        private readonly IValidator<CreateShowtimeRequest> _validator;
        private readonly ILogger<ShowtimesController> _logger;

        public ShowtimesController(IShowtimeService service, IValidator<CreateShowtimeRequest> validator, ILogger<ShowtimesController> logger)
        {
            _service = service;
            _validator = validator;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await _service.GetAllAsync(cancellationToken);
            return Ok(new GetAllShowtimesResponse
            {
                Showtimes = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {            
            if (id <= 0)
            {
                _logger.LogInformation("Invalid id: {id} in request: {request}", id , HttpContext);
                return BadRequest(new ErrorResponse()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request."
                });
            }

            var result = await _service.GetByIdAsync(id, cancellationToken);
            return Ok(result);
        }
 
        [HttpPost("create")]
        public async Task<IActionResult> CreateShowtimeAsync([FromBody] CreateShowtimeRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                // TODO: Unpack validationResult.Errors to error object response
                return BadRequest(new ErrorResponse()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request."
                });
            }

            var createdShowtimeId = await _service.CreateAsync(request.AuditoriumId, request.ImdbMovieId, request.SessionDate.Value, cancellationToken);
            return StatusCode((int)HttpStatusCode.Created, new CreateShowtimeResponse()
            {
                Id = createdShowtimeId
            });
        }
    }
}