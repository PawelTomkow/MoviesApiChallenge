using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Core.Models;

namespace ApiApplication.Core.Services
{
    public interface IReservationService
    {
        Task<List<Seat>> GetReservedSeatsByShowtimeId(int showtimeId, CancellationToken cancellationToken);
    }
}