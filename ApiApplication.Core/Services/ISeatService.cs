using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Core.Models;

namespace ApiApplication.Core.Services
{
    public interface ISeatService
    {
        Task<List<SeatWithStatus>> GetWithStatusByShowtimeIdAsync(int showtimeId, CancellationToken cancellationToken);
        Task<List<Seat>> GetSeatsByAuditoriumIdAsync(int auditoriumId, CancellationToken cancellationToken);
    }
}