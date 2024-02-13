using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        public MoviesController()
        {
            
        }
        
        [HttpGet("all")]
        public async Task<IActionResult> GetRepertoiresAsync()
        {
            throw new NotImplementedException();
        }
    }
}