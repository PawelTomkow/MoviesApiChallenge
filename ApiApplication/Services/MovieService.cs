using System.Collections.Generic;
using System.Linq;
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
        
        public async Task<List<Movie>> GetAll()
        {
            var movies = await _apiClient.GetAllAsync();
            if (movies.ShowResponses.Any())
            {
                return _mapper.Map<List<Movie>>(movies.ShowResponses);
            }

            return new List<Movie>();
        }
    }
}