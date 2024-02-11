using System.Threading.Tasks;
using ApiApplication.Clients.Contracts;

namespace ApiApplication.Clients
{
    public class ApiClientHttp : IApiClient
    {
        public async Task<ShowResponse> GetByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ShowResponse> SearchAsync(string text)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ShowListResponse> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}