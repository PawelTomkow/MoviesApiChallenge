using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace ApiApplication.HttpTests
{
    public class Tests
    {
        private TestServer _server;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Test]
        public async Task GetReservationById_Should_Return200()
        {
            //Arrange
            const int reservationId = 1;
            
            //Act
            var response = await _client.GetAsync($"/api/reservations/{reservationId}");

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}