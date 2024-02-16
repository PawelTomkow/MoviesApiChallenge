using System.Collections.Generic;
using System.Threading.Tasks;
using ApiApplication.Core.Models;

namespace ApiApplication.Core.Services
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAll();
    }
}