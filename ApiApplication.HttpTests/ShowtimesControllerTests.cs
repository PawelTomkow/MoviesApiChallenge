using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Reservations;
using ApiApplication.Controllers.Contracts.Showtimes;
using ApiApplication.HttpTests.Base;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace ApiApplication.HttpTests
{
    public class ShowtimesControllerTests : HttpRequestCreator
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
        public async Task CreateShowtimeAsync_ShouldReturn201AndShowtimeId_WhenCreateShowtimeRequestIsCorrect()
        {
            //Arrange
            var request = new CreateShowtimeRequest
            {
                AuditoriumId = 1,
                MovieId = "1",
                SessionDate = new DateTime(2024, 2, 12, 1,1,1)
            };
            var requestBody = SerializeToStringContent(request);
            
            //Act
            var result = await _client.PostAsync("/api/showtimes/create", requestBody);

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.Created);

            var resultId = await DeserializeHttpContentAsync<CreateShowtimeResponse>(result);
            resultId.Should().NotBeNull();
        }

        [TestCase(1, null)]
        [TestCase(1, "")]
        [TestCase(0, "1")]
        [TestCase(-1, "1")]
        public async Task CreateShowtimeAsync_ShouldReturn400_WhenCreateShowtimeRequestIsInvalid(int auditoriumId, string movieId)
        {
            //Arrange
            var request = new CreateShowtimeRequest
            {
                AuditoriumId = auditoriumId,
                MovieId = movieId,
                SessionDate = new DateTime(2024, 2, 12, 1,1,1)
            };
            var requestBody = SerializeToStringContent(request);
            
            //Act
            var result = await _client.PostAsync("/api/showtimes/create", requestBody);
        
            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
            var responseErrorMessage = await DeserializeHttpContentAsync<ErrorResponse>(result);
            responseErrorMessage.Should().NotBeNull();
            responseErrorMessage.ErrorMessage.Should().Be("Invalid request.");
            responseErrorMessage.StatusCode.Should().Be((int)result.StatusCode);
        }

        [Test]
        public async Task CreateShowtimeAsync_ShouldReturn400AndReservationId_WhenAuditoriumIdAndMovieIdIsCorrectButSessionDateIsNull()
        {
            //Arrange
            var request = new CreateShowtimeRequest
            {
                AuditoriumId = 1,
                MovieId = "1",
                SessionDate = null
            };
            var requestBody = SerializeToStringContent(request);
            
            //Act
            var result = await _client.PostAsync("/api/showtimes/create", requestBody);

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
            var responseErrorMessage = await DeserializeHttpContentAsync<ErrorResponse>(result);
            responseErrorMessage.Should().NotBeNull();
            responseErrorMessage.ErrorMessage.Should().Be("Invalid request.");
            responseErrorMessage.StatusCode.Should().Be((int)result.StatusCode);
        }
        
        [Test]
        public async Task GetShowtimesAsync_ShouldReturn200_WhenExitingReservationIdExist()
        {
            //Arrange
            //Arrange
            var request = new CreateShowtimeRequest
            {
                AuditoriumId = 1,
                MovieId = "1",
                SessionDate = new DateTime(2024, 2, 12, 1,1,1)
            };

            var responseAsObject = await CreateShowtimeAsync(request);

            //Act
            var response = await _client.GetAsync($"/api/showtimes/{responseAsObject.Id}");

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        private async Task<CreateShowtimeResponse> CreateShowtimeAsync(CreateShowtimeRequest request)
        {

            var requestBodyAsStringContent = SerializeToStringContent(request);
            
            var responseCreatedReservation = await _client.PostAsync("/api/showtimes/create", requestBodyAsStringContent);
            responseCreatedReservation.EnsureSuccessStatusCode();
            
            var responseAsObject = await DeserializeHttpContentAsync<CreateShowtimeResponse>(responseCreatedReservation);
            return responseAsObject;
        }
    }
}