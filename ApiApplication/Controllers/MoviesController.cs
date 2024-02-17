using System.Threading.Tasks;
using ApiApplication.Clients;
using ApiApplication.Controllers.Contracts.Movies;
using ApiApplication.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _service;

        public MoviesController(IMovieService service)
        {
            _service = service;
        }
        
        [HttpGet("all")]
        public async Task<IActionResult> GetMoviesAsync()
        {
            var result = await _service.GetAllAsync();
            return Ok(new MoviesResponse
            {
                Movies = result
            });
        }
    }
}