using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Core.Models;

namespace ApiApplication.Core.Services
{
    public interface IShowtimeService
    {
        Task<List<Showtime>> GetAllAsync(CancellationToken cancellationToken);
        Task<Showtime> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<int> CreateAsync(int auditoriumId, string imdbMovieId, DateTime sessionDate,
            CancellationToken cancellationToken);

        Task<SoldSeats> GetByIdWithAuditoriumIdAsync(int showtimeId, CancellationToken cancellationToken);
    }
}