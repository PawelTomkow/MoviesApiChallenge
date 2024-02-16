using System;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/auditorium")]
    public class AuditoriumsController : ControllerBase
    {
        public AuditoriumsController()
        {
            
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
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