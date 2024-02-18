using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Reservations;
using ApiApplication.Controllers.Contracts.Tickets;
using ApiApplication.Core.Models;
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
        private TestDataDbSeeder _testDataDbSeeder;

        [SetUp]
        public void Setup()
        {
            (_server, _client) = CreateTestServerSetup();
            _testDataDbSeeder = BuildTestDataDbSeeder(_server);
        }
        
        [TearDown]
        public void TearDown()
        {
            _testDataDbSeeder.Clear();
            _client.Dispose();
            _server.Dispose();
        }
        
        [Test]
        public async Task BuyTicketAsync_ShouldReturn201AndReservationId_WhenCreateReservationRequestIsCorrect()
        {
            //Arrange
            _testDataDbSeeder.AddNewShowtimeToDatabase();
            var requestReservationBody = new CreateReservationRequest
            {
                ShowtimeId = 1,
                Seats = new List<Seat> {new Seat{SeatNumber = 1, Row = 1}},
                AuditoriumId = 1
            };

            var responseAsObject = await CreateReservationAsync(requestReservationBody);
            var response = await _client.GetAsync($"/api/reservations/{responseAsObject.Id}");
            var responseObj = await DeserializeHttpContentAsync<Reservation>(response);
            var request = new CreateTicketRequest
            {
                ReservationId = responseObj.Id
            };
            var requestBody = SerializeToStringContent(request);
            
            //Act
            var result = await _client.PostAsync("api/tickets/create", requestBody);

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
            var request = new CreateTicketRequest
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
        public async Task BuyTicketAsync_ShouldReturn404_WhenReservationIdNotExist()
        {
            //Arrange
            const string expectedTicketId = "923321";
            var request = new BuyTicketRequest
            {
                TicketId = expectedTicketId
            };
            var requestBody = SerializeToStringContent(request);
            
            //Act
            var result = await _client.PostAsync("api/tickets/buy", requestBody);
        
            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
            var responseErrorMessage = await DeserializeHttpContentAsync<ErrorResponse>(result);
            responseErrorMessage.Should().NotBeNull();
            responseErrorMessage.Message.Should().Be($"Ticket with ticketId: {expectedTicketId} not found.");
            responseErrorMessage.StatusCode.Should().Be((int)result.StatusCode);
        }
        
        private async Task<TicketResponse> CreateTicketAsync(CreateTicketRequest request)
        {

            var requestBodyAsStringContent = SerializeToStringContent(request);
            
            var responseCreatedReservation = await _client.PostAsync("api/tickets/create", requestBodyAsStringContent);
            responseCreatedReservation.EnsureSuccessStatusCode();
            
            var responseAsObject = await DeserializeHttpContentAsync<TicketResponse>(responseCreatedReservation);
            return responseAsObject;
        }
        
        private async Task<TicketResponse> BuyTicketAsync(BuyTicketRequest request)
        {
            var requestBodyAsStringContent = SerializeToStringContent(request);
            
            var responseCreatedReservation = await _client.PostAsync("api/tickets/buy", requestBodyAsStringContent);
            responseCreatedReservation.EnsureSuccessStatusCode();
            
            var responseAsObject = await DeserializeHttpContentAsync<TicketResponse>(responseCreatedReservation);
            return responseAsObject;
        }
        
        private async Task<CreateReservationResponse> CreateReservationAsync(CreateReservationRequest request)
        {

            var requestBodyAsStringContent = SerializeToStringContent(request);
            
            var responseCreatedReservation = await _client.PostAsync("/api/reservations/create", requestBodyAsStringContent);
            responseCreatedReservation.EnsureSuccessStatusCode();
            
            var responseAsObject = await DeserializeHttpContentAsync<CreateReservationResponse>(responseCreatedReservation);
            return responseAsObject;
        }
    }
}