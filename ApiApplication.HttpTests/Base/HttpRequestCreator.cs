using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApiApplication.Clients;
using ApiApplication.Database;
using ApiApplication.HttpTests.Base.FixtureExtensions;
using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NUnit.Framework;

namespace ApiApplication.HttpTests.Base
{
    public abstract class HttpRequestCreator
    {
        protected readonly Fixture Fixture;

        public HttpRequestCreator()
        {
            Fixture = new Fixture();
            Fixture.Customizations.Add(new IdPropertyGenerator());
        }
        
        public async Task<T> DeserializeHttpContentAsync<T>(HttpResponseMessage responseCreatedReservation)
        {
            var responseAsString = await responseCreatedReservation.Content.ReadAsStringAsync();
            var responseAsObject = JsonSerializer.Deserialize<T>(responseAsString, 
                new JsonSerializerOptions {PropertyNameCaseInsensitive = true});
            return responseAsObject;
        }

        public StringContent SerializeToStringContent<T>(T requestBody)
        {
            return new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        }

        public (TestServer, HttpClient) CreateTestServerSetup()
        {
            var appSettingsConfiguration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            
            var configuration = new Dictionary<string, string>
            {
                {"IsTest", "true"}
            };
            
            var server = new TestServer(new WebHostBuilder()
                .UseConfiguration(appSettingsConfiguration)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(configuration);
                })
                .UseStartup<Startup>()
                .ConfigureTestServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                             typeof(IApiClient)); // Replace DbContext with the service you want to remove
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddScoped<IApiClient, TestApiClient>();
                })
            );
            var client = server.CreateClient();

            return (server, client);
        }
        
        protected TestDataDbSeeder BuildTestDataDbSeeder(TestServer server)
        {
            var mapper = server.Services.GetService<IMapper>();
            var dbContext = server.Services.GetService<CinemaContext>();
            if (mapper is null || dbContext is null)
            {
                throw new ArgumentException("Mapper and DbContext cannot be null.");
            }
            
            return new TestDataDbSeeder(mapper, dbContext);
        }
    }
}