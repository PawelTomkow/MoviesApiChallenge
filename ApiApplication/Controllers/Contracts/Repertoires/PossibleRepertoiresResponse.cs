using System.Collections.Generic;
using ApiApplication.Core.Models;

namespace ApiApplication.Controllers.Contracts.Repertoires
{
    public class PossibleRepertoiresResponse
    {
        public IEnumerable<Movie> Repertoires { get; set; }
    }
}