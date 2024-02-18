using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Seats;
using ApiApplication.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/seats")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _service;
        private readonly ILogger<SeatsController> _logger;

        public SeatsController(ISeatService service, ILogger<SeatsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{auditoriumId}")]
        public async Task<IActionResult> GetSeatsByAuditoriumIdAsync(int auditoriumId, CancellationToken cancellationToken)
        {
            if (auditoriumId <= 0)
            {
                _logger.LogInformation("Invalid id: {id} in request: {request}", auditoriumId , HttpContext);
                return BadRequest(new ErrorResponse()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request."
                });
            }
            
            var result = await _service.GetSeatsByAuditoriumIdAsync(auditoriumId, cancellationToken);
            return Ok(new GetSeatsByAuditoriumIdResponse
            {
                Seats = result,
                AuditoriumId = auditoriumId
            } );
        }

        [HttpGet("status/{showtimeId}")]
        public async Task<IActionResult> GetShowtimeSeatsWithStatusAsync(int showtimeId,
            CancellationToken cancellationToken)
        {
            if (showtimeId <= 0)
            {
                _logger.LogInformation("Invalid id: {id} in request: {request}", showtimeId , HttpContext);
                return BadRequest(new ErrorResponse()
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid request."
                });
            }
            
            var seatWithStatusList =
                await _service.GetWithStatusByShowtimeIdAsync(showtimeId, cancellationToken);
            return Ok(new GetShowtimeSeatsWithStatusResponse
            {
                SeatsWithStatus = seatWithStatusList 
            });
        }
    }
}