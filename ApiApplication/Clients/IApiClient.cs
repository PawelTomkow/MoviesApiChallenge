using System.Threading.Tasks;
using ApiApplication.Clients.Contracts;
using ProtoDefinitions;

namespace ApiApplication.Clients
{
    public interface IApiClient
    {
        Task<ShowResponse> GetByIdAsync(string id);
        Task<ShowResponse> SearchAsync(string text);
        Task<ShowListResponse> GetAllAsync();
    }
}