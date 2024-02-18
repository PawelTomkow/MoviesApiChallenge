using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Clients.Cache;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ICacheRepository _cacheRepository;
        private readonly ILogger<ICacheRepository> _logger;
        private readonly IMapper _mapper;

        public ReservationService(ICacheRepository cacheRepository, ILogger<ICacheRepository> logger, IMapper mapper)
        {
            _cacheRepository = cacheRepository;
            _logger = logger;
            _mapper = mapper;
        }
        
        public Task<List<Seat>> GetReservedSeatsByShowtimeId(int showtimeId, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<Seat>());
        }
    }
}