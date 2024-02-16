using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Reservations;
using ApiApplication.Controllers.Contracts.Tickets;
using ApiApplication.HttpTests.Base;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace ApiApplication.HttpTests
{
    public class TicketsControllerTests : HttpRequestCreator
    {
        private TestServer _server;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            (_server, _client) = CreateTestServerSetup();
        }
        
        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _server.Dispose();
        }
        
        [Test]
        public async Task BuyTicketAsync_ShouldReturn201AndReservationId_WhenCreateReservationRequestIsCorrect()
        {
            //Arrange
            var request = new BuyTicketRequest
            {
                ReservationId = "1"
            };
            var requestBody = SerializeToStringContent(request);
            
            //Act
            var result = await _client.PostAsync("api/tickets/buy", requestBody);

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.Created);

            var resultId = await DeserializeHttpContentAsync<CreateReservationResponse>(result);
            resultId.Should().NotBeNull();
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task BuyTicketAsync_ShouldReturn400_WhenCreateReservationRequestIsInvalid(string reservationId)
        {
            //Arrange
            var request = new BuyTicketRequest
            {
                ReservationId = reservationId
            };
            var requestBody = SerializeToStringContent(request);
            
            //Act
            var result = await _client.PostAsync("api/tickets/buy", requestBody);
        
            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
            var responseErrorMessage = await DeserializeHttpContentAsync<ErrorResponse>(result);
            responseErrorMessage.Should().NotBeNull();
            responseErrorMessage.Message.Should().Be("Invalid request.");
            responseErrorMessage.StatusCode.Should().Be((int)result.StatusCode);
        }
        
        [Test]
        public async Task GetShowtimesAsync_ShouldReturn200_WhenExitingReservationIdExist()
        {
            //Arrange
            //Arrange
            var request = new BuyTicketRequest
            {
                ReservationId = "1"
            };

            var responseAsObject = await BuyTicketAsync(request);

            //Act
            var response = await _client.GetAsync($"/api/showtimes/{responseAsObject.TicketId}");

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task BuyTicketAsync_ShouldReturn404_WhenReservationIdNotExist()
        {
            //Arrange
            var request = new BuyTicketRequest
            {
                ReservationId = "923321"
            };
            var requestBody = SerializeToStringContent(request);
            
            //Act
            var result = await _client.PostAsync("api/tickets/buy", requestBody);
        
            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
            var responseErrorMessage = await DeserializeHttpContentAsync<ErrorResponse>(result);
            responseErrorMessage.Should().NotBeNull();
            responseErrorMessage.Message.Should().Be("Reservation not found.");
            responseErrorMessage.StatusCode.Should().Be((int)result.StatusCode);
        }
        
        private async Task<TicketResponse> BuyTicketAsync(BuyTicketRequest request)
        {

            var requestBodyAsStringContent = SerializeToStringContent(request);
            
            var responseCreatedReservation = await _client.PostAsync("api/tickets/buy", requestBodyAsStringContent);
            responseCreatedReservation.EnsureSuccessStatusCode();
            
            var responseAsObject = await DeserializeHttpContentAsync<TicketResponse>(responseCreatedReservation);
            return responseAsObject;
        }
    }
}