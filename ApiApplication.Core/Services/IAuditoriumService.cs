using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Core.Models;

namespace ApiApplication.Core.Services
{
    public interface IAuditoriumService
    {
        Task<List<Auditorium>> GetAllAsync(CancellationToken cancellationToken, bool withSeats = false);
        Task<Auditorium> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}