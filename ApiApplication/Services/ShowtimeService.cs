using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Clients;
using ApiApplication.Clients.Contracts;
using ApiApplication.Core.Exceptions;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IShowtimesRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ShowtimeService> _logger;
        private readonly IApiClient _apiClient;
        private readonly IAuditoriumService _auditoriumService;
        private readonly IMovieService _movieService;

        public ShowtimeService(IShowtimesRepository repository, 
            IMapper mapper, 
            ILogger<ShowtimeService> logger,
            IApiClient apiClient,
            IAuditoriumService auditoriumService,
            IMovieService movieService)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _apiClient = apiClient;
            _auditoriumService = auditoriumService;
            _movieService = movieService;
        }
        
        public async Task<List<Showtime>> GetAllAsync(CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync(filter: null, cancellationToken);
            var domainObj = _mapper.Map<List<Showtime>>(entities);
            return domainObj;
        }

        public async Task<Showtime> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity is null)
            {
                _logger.LogInformation("Showtime with id: {id} not found.", id);
                throw new ResourceNotFoundException(typeof(Showtime), nameof(id), id.ToString());
            }
            var domainObj = _mapper.Map<Showtime>(entity);
            return domainObj;
        }

        public async Task<int> CreateAsync(int auditoriumId, string imdbMovieId, DateTime sessionDate, CancellationToken cancellationToken)
        {
            var responseApiClient = await _apiClient.GetByIdAsync(imdbMovieId);
            if (responseApiClient is null)
            {
                throw new ResourceNotFoundException(typeof(ShowResponse), nameof(imdbMovieId), imdbMovieId);
            }

            var movie = _mapper.Map<Movie>(responseApiClient);
            var auditorium = await _auditoriumService.GetByIdAsync(auditoriumId, cancellationToken);
            if (auditorium is null)
            {
                throw new ResourceNotFoundException(typeof(Auditorium), nameof(auditoriumId), auditoriumId.ToString());
            }

            var createdShowtime = await _repository.CreateShowtime(new ShowtimeEntity
            {
                AuditoriumId = auditorium.Id,
                SessionDate = sessionDate,
                Movie = new MovieEntity
                {
                    Title = movie.Title,
                    ImdbId = movie.ImdbId,
                    Stars = movie.Rank,
                    ReleaseDate = new DateTime(int.Parse(movie.Year), 1, 1)
                }
            }, cancellationToken);

            return createdShowtime?.Id ?? 0;
        }

        public async Task<SoldSeats> GetByIdWithAuditoriumIdAsync(int showtimeId, CancellationToken cancellationToken)
        {
            var showtimeEntity = await _repository.GetByIdAsync(showtimeId, cancellationToken);
            if (showtimeEntity is null)
            {
                throw new ResourceNotFoundException(typeof(ShowtimeEntity), nameof(showtimeId), showtimeId.ToString());
            }

            var showtimeTickets = await _repository.GetWithTicketsByIdAsync(showtimeId, cancellationToken);
            var soldSeats = showtimeTickets.Tickets
                .Select(x => x.Seats)
                .Select(x => x.ToList())
                .SelectMany(list => list)
                .ToList();
            
            return new SoldSeats
                {
                    Showtime = _mapper.Map<Showtime>(showtimeEntity), 
                    AuditoriumId = showtimeEntity.AuditoriumId,
                    Seats = _mapper.Map<List<Seat>>(soldSeats)
                };
        }
    }
}