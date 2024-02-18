using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Core.Models;

namespace ApiApplication.Core.Services
{
    public interface IReservationService
    {
        Task<Reservation> GetByIdAsync(string reservationId, CancellationToken cancellationToken);
        Task<Reservation> CreateAsync(int auditoriumId, int showtimeId, List<Seat> seats,
            CancellationToken cancellationToken);
        Task<List<Seat>> GetReservedSeatsByShowtimeId(int showtimeId, CancellationToken cancellationToken);

    }
}