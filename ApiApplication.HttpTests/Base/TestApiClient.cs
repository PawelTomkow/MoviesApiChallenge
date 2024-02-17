using System.Collections.Generic;
using System.Threading.Tasks;
using ApiApplication.Clients;
using ApiApplication.Clients.Contracts;

namespace ApiApplication.HttpTests.Base
{
    public static class TestApiClientValues
    {
        public const string IdMovie = "tt1";
    }
    
    public class TestApiClient : IApiClient
    {
        
        
        public Task<ShowResponse> GetByIdAsync(string id)
        {
            return Task.FromResult(new ShowResponse()
            {
                Year = "2011",
                Crew = "aksfdhask",
                Id = TestApiClientValues.IdMovie,
                Image = "http://baseaddress/image",
                Rank = "10",
                Title = "Welcome in company :)",
                FullTitle = "Hello",
                ImDbRating = "10",
                ImDbRatingCount = "10"
            });
        }

        public Task<ShowListResponse> SearchAsync(string text)
        {
            return Task.FromResult(new ShowListResponse
            {
                ShowResponses = new List<ShowResponse>()
                {
                    new ShowResponse()
                    {
                        Year = "2011",
                        Crew = "aksfdhask",
                        Id = "tt1",
                        Image = "http://baseaddress/image",
                        Rank = "10",
                        Title = "Welcome in company :)",
                        FullTitle = "Hello",
                        ImDbRating = "10",
                        ImDbRatingCount = "10"
                    }
                }
            });
        }

        public Task<ShowListResponse> GetAllAsync()
        {
            return Task.FromResult(new ShowListResponse
            {
                ShowResponses = new List<ShowResponse>()
                {
                    new ShowResponse()
                    {
                        Year = "2011",
                        Crew = "aksfdhask",
                        Id = "tt1",
                        Image = "http://baseaddress/image",
                        Rank = "10",
                        Title = "Welcome in company :)",
                        FullTitle = "Hello",
                        ImDbRating = "10",
                        ImDbRatingCount = "10"
                    }
                }
            });
        }
    }
}