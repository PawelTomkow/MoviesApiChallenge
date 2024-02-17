using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Core.Models;

namespace ApiApplication.Core.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAllAsync();
        Task<Movie> GetByImdbIdAsync(string imdbMovieId, CancellationToken cancellationToken);
    }
}