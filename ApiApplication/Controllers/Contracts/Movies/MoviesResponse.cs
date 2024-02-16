using System.Collections.Generic;
using ApiApplication.Core.Models;

namespace ApiApplication.Controllers.Contracts.Movies
{
    public class MoviesResponse
    {
        public IEnumerable<Movie> Movies { get; set; }
    }
}