using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Clients.Cache;
using ApiApplication.Core.Exceptions;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Services
{
    public class SeatService : ISeatService
    {
        private readonly IAuditoriumService _auditoriumService;
        private readonly IShowtimeService _showtimeService;
        private readonly ILogger<SeatService> _logger;
        private readonly IReservationService _reservationService;
        private readonly ICacheRepository _cache;

        public SeatService(IAuditoriumService auditoriumService, 
            IShowtimeService showtimeService, 
            ILogger<SeatService> logger,
            IReservationService reservationService,
            ICacheRepository cache)
        {
            _auditoriumService = auditoriumService;
            _showtimeService = showtimeService;
            _logger = logger;
            _reservationService = reservationService;
            _cache = cache;
        }
        
        public async Task<List<SeatWithStatus>> GetWithStatusByShowtimeIdAsync(int showtimeId, CancellationToken cancellationToken)
        {
            var soldSeats = await _showtimeService.GetByIdWithAuditoriumIdAsync(showtimeId, cancellationToken);
            if (soldSeats is null)
            {
                throw new ResourceNotFoundException(typeof(SoldSeats), nameof(showtimeId), showtimeId.ToString());
            }

            var auditorium = await _auditoriumService.GetByIdAsync(soldSeats.AuditoriumId, cancellationToken);
            var allSeats = auditorium.Seats.OrderBy(x=> x.Row).ThenBy(x=>x.SeatNumber);

            var listWithStatus = allSeats
                .Select(seat => new SeatWithStatus { Seat = seat, Status = SeatStatus.Free })
                .ToList();

            foreach (var soldSeat in soldSeats.Seats)
            {
                listWithStatus.FirstOrDefault(x =>
                    x.Seat.SeatNumber == soldSeat.SeatNumber && x.Seat.Row == soldSeat.Row)!.Status = SeatStatus.Sold;
            }

            var reservedSeats = await _reservationService.GetReservedSeatsByShowtimeId(showtimeId, cancellationToken);

            foreach (var reservedSeat in reservedSeats)
            {
                listWithStatus.FirstOrDefault(x =>
                        x.Seat.SeatNumber == reservedSeat.SeatNumber && x.Seat.Row == reservedSeat.Row)!.Status =
                    SeatStatus.Reserved;
            }

            await SaveToCacheAsync(listWithStatus, showtimeId);

            return listWithStatus;
        }

        public async Task<List<Seat>> GetSeatsByAuditoriumIdAsync(int auditoriumId, CancellationToken cancellationToken)
        {
            var auditorium = await _auditoriumService.GetByIdAsync(auditoriumId, cancellationToken);
            if (auditorium is null)
            {
                throw new ResourceNotFoundException(typeof(Auditorium), nameof(auditoriumId), auditoriumId.ToString());
            }

            return auditorium.Seats;
        }

        public async Task SaveToCacheAsync(List<SeatWithStatus> withStatusList, int showtimeId)
        {
            if (withStatusList != null && !withStatusList.Any())
            {
                foreach (var seatStatus in withStatusList)
                {
                    await _cache.SetValueAsync(
                        SeatStatusKeyBuilder(showtimeId, seatStatus),
                        seatStatus.Status);
                }
            }
        }

        private static string SeatStatusKeyBuilder(int showtimeId, SeatWithStatus seatStatus)
        {
            return SeatStatusKeyBuilder(showtimeId, seatStatus.Seat.SeatNumber, seatStatus.Seat.Row);
        }
        
        private static string SeatStatusKeyBuilder(int showtimeId, short seatNumber, short row)
        {
            return $"SeatStatus-{showtimeId}-{seatNumber}-{row}";
        }

        public async Task<SeatStatus> GetStatusAsync(int showtimeId, short seatNumber, short row, CancellationToken cancellationToken)
        {
            var result = await _cache.GetValueAsync<SeatStatus>(SeatStatusKeyBuilder(showtimeId, seatNumber, row));
            if (result != SeatStatus.Unknown)
            {
                return result;
            }
            
            _ = await GetWithStatusByShowtimeIdAsync(showtimeId, cancellationToken);
            return await _cache.GetValueAsync<SeatStatus>(SeatStatusKeyBuilder(showtimeId, seatNumber, row));

        }
    }
}