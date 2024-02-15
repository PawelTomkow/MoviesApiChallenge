using System.Threading.Tasks;
using ApiApplication.Clients.Contracts;

namespace ApiApplication.Clients
{
    public class ApiClientHttp : IApiClient
    {
        public Task<ShowResponse> GetByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ShowListResponse> SearchAsync(string text)
        {
            throw new System.NotImplementedException();
        }

        public Task<ShowListResponse> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}