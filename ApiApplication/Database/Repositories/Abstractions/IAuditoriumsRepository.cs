using System.Collections.Generic;
using ApiApplication.Database.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Database.Repositories.Abstractions
{
    public interface IAuditoriumsRepository
    {
        Task<AuditoriumEntity> GetAsync(int auditoriumId, CancellationToken cancel);
        Task<List<AuditoriumEntity>> GetAllAsync(CancellationToken cancellationToken, bool withSeats = false);
    }
}