using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Reservations;
using ApiApplication.Core.Models;
using ApiApplication.HttpTests.Base;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace ApiApplication.HttpTests
{
    public class ReservationsControllerTests : HttpRequestCreator
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
        public async Task CreateReservationAsync_ShouldReturn201AndReservationId_WhenCreateReservationRequestIsCorrect()
        {
            //Arrange
            var request = new CreateReservationRequest
            {
                ShowtimeId = "1",
                Seats = new List<Seat> {new Seat{SeatNumber = 1, Row = 1}}
            };
            var requestBody = SerializeToStringContent(request);
            
            //Act
            var result = await _client.PostAsync("/api/reservations/create", requestBody);

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.Created);

            var resultId = await DeserializeHttpContentAsync<CreateReservationResponse>(result);
            resultId.Should().NotBeNull();
        }

        [TestCase("1", null,1)]
        [TestCase("", null,-1)]
        [TestCase(null, null,-1)]
        [TestCase(null, null,1)]
        [TestCase("1",null,-1)]
        public async Task CreateReservationAsync_ShouldReturn400_WhenCreateReservationRequestIsInvalid(string idShowtime, List<Seat> seats, int auditoriumId)
        {
            //Arrange
            var request = new CreateReservationRequest
            {
                ShowtimeId = idShowtime,
                Seats = seats,
                AuditoriumId = auditoriumId
            };
            var requestBody = SerializeToStringContent(request);
            
            //Act
            var result = await _client.PostAsync("/api/reservations/create", requestBody);
        
            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
            var responseErrorMessage = await DeserializeHttpContentAsync<ErrorResponse>(result);
            responseErrorMessage.Should().NotBeNull();
            responseErrorMessage.Message.Should().Be("Invalid request.");
            responseErrorMessage.StatusCode.Should().Be((int)result.StatusCode);
        }

        [Test]
        public async Task CreateReservationAsync_ShouldReturn400AndReservationId_WhenSeatsAreCorrectButIdShowtimeIsEmpty()
        {
            //Arrange
            var request = new CreateReservationRequest
            {
                ShowtimeId = "",
                Seats = new List<Seat> {new Seat{SeatNumber = 1, Row = 1}}
            };
            var requestBody = SerializeToStringContent(request);
            
            //Act
            var result = await _client.PostAsync("/api/reservations/create", requestBody);

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
            var responseErrorMessage = await DeserializeHttpContentAsync<ErrorResponse>(result);
            responseErrorMessage.Should().NotBeNull();
            responseErrorMessage.Message.Should().Be("Invalid request.");
            responseErrorMessage.StatusCode.Should().Be((int)result.StatusCode);
        }

        [Test]
        public async Task CreateReservationAsync_ShouldReturn409_WhenReservationSameSeat()
        {
            //Arrange
            var requestBody = new CreateReservationRequest
            {
                ShowtimeId = "1",
                AuditoriumId = 1,
                Seats = new List<Seat> {new Seat{SeatNumber = 1, Row = 1}}
            };
            var requestBodyAsStringContent = SerializeToStringContent(requestBody);
            var responseAsObject = await CreateReservationAsync(requestBody);

            //Act
            var response = await _client.PostAsync("/api/reservations/create", requestBodyAsStringContent);

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            
            var responseErrorMessage = await DeserializeHttpContentAsync<ErrorResponse>(response);
            responseErrorMessage.Should().NotBeNull();
            responseErrorMessage.Message.Should().Be("This place cannot be booked because it has been booked by someone else.");
            responseErrorMessage.StatusCode.Should().Be((int)response.StatusCode);
        }
        
        [Test]
        public async Task GetReservationByIdAsync_ShouldReturn200_WhenExitingReservationIdExist()
        {
            //Arrange
            var requestBody = new CreateReservationRequest
            {
                ShowtimeId = "1",
                Seats = new List<Seat> {new Seat{SeatNumber = 1, Row = 1}}
            };

            var responseAsObject = await CreateReservationAsync(requestBody);

            //Act
            var response = await _client.GetAsync($"/api/reservations/{responseAsObject.Id}");

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
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