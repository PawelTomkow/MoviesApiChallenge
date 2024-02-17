using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Clients;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using AutoMapper;

namespace ApiApplication.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMapper _mapper;
        private readonly IApiClient _apiClient;

        public MovieService(IMapper mapper, IApiClient apiClient)
        {
            _mapper = mapper;
            _apiClient = apiClient;
        }
        
        public async Task<List<Movie>> GetAllAsync()
        {
            var movies = await _apiClient.GetAllAsync();
            if (movies.ShowResponses.Any())
            {
                return _mapper.Map<List<Movie>>(movies.ShowResponses);
            }

            return new List<Movie>();
        }

        public async Task<Movie> GetByImdbIdAsync(string imdbMovieId, CancellationToken cancellationToken)
        {
            var entity = await _apiClient.GetByIdAsync(imdbMovieId);
            return _mapper.Map<Movie>(entity);
        }

    }
}