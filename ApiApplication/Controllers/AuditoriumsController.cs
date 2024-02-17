using System;
using System.Data;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Auditoriums;
using ApiApplication.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/auditorium")]
    public class AuditoriumsController : ControllerBase
    {
        private readonly IAuditoriumService _service;
        private readonly ILogger<AuditoriumsController> _logger;

        public AuditoriumsController(IAuditoriumService service, ILogger<AuditoriumsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await _service.GetAllAsync(cancellationToken);
            return Ok(new GetAllAuditoriumsResponse()
            {
                Auditoriums = result
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
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

            var response = await _service.GetByIdAsync(id, cancellationToken);
            return Ok(response);
        }
    }
}