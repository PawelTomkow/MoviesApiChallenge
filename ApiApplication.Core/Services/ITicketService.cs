using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Core.Models;

namespace ApiApplication.Core.Services
{
    public interface ITicketService
    {
        Task<Ticket> CreateAsync(string reservationId, CancellationToken cancellationToken);
        Task BuyAsync(string ticketId, CancellationToken cancellationToken);
        Task<Ticket> GetAsync(string id, CancellationToken cancellationToken);
    }
}