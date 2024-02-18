using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Clients.Cache;
using ApiApplication.Configurations;
using ApiApplication.Core.Exceptions;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApiApplication.Services
{
    public class SeatService : ISeatService
    {
        private readonly IAuditoriumService _auditoriumService;
        private readonly IShowtimeService _showtimeService;
        private readonly ILogger<SeatService> _logger;
        private readonly ICacheRepository _cache;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ReservationConfiguration _config;

        public SeatService(IAuditoriumService auditoriumService, 
            IShowtimeService showtimeService, 
            ILogger<SeatService> logger,
            ICacheRepository cache,
            IOptions<ReservationConfiguration> options,
            IDateTimeProvider dateTimeProvider)
        {
            _auditoriumService = auditoriumService;
            _showtimeService = showtimeService;
            _logger = logger;
            _cache = cache;
            _dateTimeProvider = dateTimeProvider;
            _config = options.Value;
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

            foreach (var seat in allSeats)
            {
                var result = await GetStatusAsync(showtimeId, seat.SeatNumber, seat.Row, cancellationToken);
                if (result is { Status: SeatStatus.Reserved })
                {
                    listWithStatus.FirstOrDefault(x =>
                        x.Seat.SeatNumber == seat.SeatNumber && x.Seat.Row == seat.Row)!.Status = SeatStatus.Reserved;
                }
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

        private async Task SetSeatStatusAsync(int showtimeId, Seat seat, SeatStatus seatStatus, CancellationToken cancellationToken)
        {
            await SetSeatStatusAsync(showtimeId, seat.SeatNumber, seat.Row, seatStatus, cancellationToken);
        }

        private async Task SetSeatStatusAsync(int showtimeId, short seatNumber, short row, SeatStatus seatStatus,
            CancellationToken cancellationToken)
        {
            var key = SeatStatusKeyBuilder(showtimeId, seatNumber, row);
            if (seatStatus == SeatStatus.Reserved)
            {
                var value = new SeatStatusWithExpirationTime
                {
                    Status = SeatStatus.Reserved,
                    CanExpired = true,
                    Expired = _dateTimeProvider.DateTimeNow.AddSeconds(_config.ExpiryTime)
                };
                await _cache.SetValueAsync(key, value);
            }

            if (seatStatus == SeatStatus.Free || seatStatus == SeatStatus.Sold)
            {
                var value = new SeatStatusWithExpirationTime
                {
                    Status = seatStatus,
                    CanExpired = false,
                    Expired = null
                };
                await _cache.SetValueAsync(key, value);
            }
        }

        public async Task BookAsync(Seat seat, int showtimeId, CancellationToken cancellationToken)
        {
            await SetSeatStatusAsync(showtimeId, seat, SeatStatus.Reserved, cancellationToken);
        }

        public async Task BookAsync(List<Seat> seats, int showtimeId, CancellationToken cancellationToken)
        {
            foreach (var seat in seats)
            {
                await SetSeatStatusAsync(showtimeId, seat, SeatStatus.Reserved, cancellationToken);
            }
        }

        public async Task<bool> CheckSeatsAreFreeAsync(List<Seat> seats, int showtimeId, CancellationToken cancellationToken)
        {
            foreach (var seat in seats)
            {
                var seatStatus = await GetStatusAsync(showtimeId, seat.SeatNumber, seat.Row, cancellationToken);
                if (seatStatus is null)
                {
                    _ = await GetWithStatusByShowtimeIdAsync(showtimeId, cancellationToken);
                    seatStatus = await GetStatusAsync(showtimeId, seat.SeatNumber, seat.Row, cancellationToken);
                    if (seatStatus is null)
                    {
                        throw new ResourceNotFoundException();
                    }
                    
                }
                if (seatStatus.Status == SeatStatus.Reserved || seatStatus.Status == SeatStatus.Sold)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task SaveToCacheAsync(List<SeatWithStatus> withStatusList, int showtimeId)
        {
            if (withStatusList != null && withStatusList.Any())
            {
                foreach (var seatStatus in withStatusList)
                {
                    var result = await _cache.GetValueAsync<SeatStatusWithExpirationTime>(SeatStatusKeyBuilder(showtimeId, seatStatus.Seat.SeatNumber, seatStatus.Seat.Row));
                    if (result is { Status: SeatStatus.Reserved, CanExpired: true } && result.Expired > _dateTimeProvider.DateTimeNow)
                    {
                        continue;
                    }

                    var statusWithExpiration = new SeatStatusWithExpirationTime
                    {
                        Status = seatStatus.Status,
                        CanExpired = false
                    };
                    await _cache.SetValueAsync(
                        SeatStatusKeyBuilder(showtimeId, seatStatus),statusWithExpiration);
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

        public async Task<SeatStatusWithExpirationTime> GetStatusAsync(int showtimeId, short seatNumber, short row, CancellationToken cancellationToken)
        {
            var key = SeatStatusKeyBuilder(showtimeId, seatNumber, row);
            var result = await _cache.GetValueAsync<SeatStatusWithExpirationTime>(key);
            if (result != null && result.Status != SeatStatus.Unknown)
            {
                if (result.CanExpired && result.Expired < _dateTimeProvider.DateTimeNow)
                {
                    await SetSeatStatusAsync(showtimeId, seatNumber, row, SeatStatus.Free, cancellationToken);
                    result.Status = SeatStatus.Free;
                    result.CanExpired = false;
                }
                
                return result;
            }

            return default;

        }
    }
}