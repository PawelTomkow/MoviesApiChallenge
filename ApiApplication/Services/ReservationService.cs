using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Clients.Cache;
using ApiApplication.Core.Comparers;
using ApiApplication.Core.Exceptions;
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
        private readonly IAuditoriumService _auditoriumService;
        private readonly ISeatService _seatService;
        private readonly IShowtimeService _showtimeService;

        public ReservationService(ICacheRepository cacheRepository, 
            ILogger<ICacheRepository> logger, 
            IMapper mapper,
            IAuditoriumService auditoriumService,
            ISeatService seatService,
            IShowtimeService showtimeService)
        {
            _cacheRepository = cacheRepository;
            _logger = logger;
            _mapper = mapper;
            _auditoriumService = auditoriumService;
            _seatService = seatService;
            _showtimeService = showtimeService;
        }
        
        public Task<List<Seat>> GetReservedSeatsByShowtimeId(int showtimeId, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<Seat>());
        }

        public async Task<Reservation> GetByIdAsync(string reservationId, CancellationToken cancellationToken)
        {
            var key = BuildKey(reservationId);
            var reservation = await _cacheRepository.GetValueAsync<Reservation>(key);
            if (reservation is null)
            {
                throw new ResourceNotFoundException(typeof(Reservation), nameof(reservationId), reservationId);
            }

            return reservation;
        }

        public async Task<Reservation> CreateAsync(int auditoriumId, int showtimeId, List<Seat> seats,
            CancellationToken cancellationToken)
        {
            var showtime = await ValidateAndReturnShowtimeAsync(auditoriumId, showtimeId, seats, cancellationToken);

            var reservation = new Reservation()
            {
                Id = Guid.NewGuid().ToString(),
                AuditoriumId = auditoriumId,
                ShowtimeId = showtimeId,
                Seats = seats,
                Movie = showtime.Movie
            };

            await _seatService.BookAsync(seats, showtimeId, cancellationToken);
            
            var key = BuildKey(reservation.Id);
            await _cacheRepository.SetValueAsync(key, reservation);
            return reservation;
        }

        private string BuildKey(string reservationId) => $"reservation-{reservationId}";
        
        private async Task<Showtime> ValidateAndReturnShowtimeAsync(int auditoriumId, int showtimeId, List<Seat> seats, CancellationToken cancellationToken)
        {
            var orderedSeats = seats.OrderBy(x => x.Row).ThenBy(x => x.SeatNumber).ToArray();
            if (orderedSeats.Length > 1)
            {
                for (var i=0; i<orderedSeats.Length -1; i++)
                {
                    if (!(orderedSeats[i].Row == orderedSeats[i + 1].Row &&
                        (orderedSeats[i].SeatNumber + 1  == orderedSeats[i + 1].SeatNumber|| orderedSeats[i].SeatNumber - 1 == orderedSeats[i + 1].SeatNumber)))
                    {
                        throw new ReservationSeatException("Reserved seats should be next to each other.");
                    }
                }
            }
            var auditorium = await _auditoriumService.GetByIdAsync(auditoriumId, cancellationToken);
            if (auditorium is null)
            {
                throw new ResourceNotFoundException(typeof(Auditorium), nameof(auditoriumId), auditoriumId.ToString());
            }

            var isShowtimeIdAssignedToAuditorium = auditorium.Showtimes.FirstOrDefault(x => x.Id == showtimeId) is null;
            if (!isShowtimeIdAssignedToAuditorium)
            {
                throw new ResourceNotFoundException(typeof(Showtime), nameof(showtimeId), showtimeId.ToString());
            }

            var showtime = await _showtimeService.GetByIdAsync(showtimeId, cancellationToken);

            var isSeatsInAuditorium = seats.All(item => auditorium.Seats.Contains(item, new SeatEqualityComparer()));
            if (!isSeatsInAuditorium)
            {
                throw new ResourceNotFoundException(typeof(List<Seat>), nameof(seats), JsonSerializer.Serialize(seats));
            }

            var isSeatsSoldOrBooked = await _seatService.CheckSeatsAreFreeAsync(seats, showtimeId, cancellationToken);
            if (!isSeatsSoldOrBooked)
            {
                throw new ReservationException("This place cannot be booked because it has been booked or buy by someone else.");
            }

            return showtime;
        }
    }
}