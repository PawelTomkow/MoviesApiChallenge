using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Reservations;
using ApiApplication.Controllers.Contracts.Showtimes;
using ApiApplication.Core.Models;
using ApiApplication.HttpTests.Base;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace ApiApplication.HttpTests
{
    public class ShowtimesControllerTests : HttpRequestCreator
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
        public async Task CreateShowtimeAsync_ShouldReturn201AndShowtimeId_WhenCreateShowtimeRequestIsCorrect()
        {
            //Arrange
            const int auditoriumId = 11;
            const string ImdbId = TestApiClientValues.IdMovie;
            var auditorium = Fixture.Create<Auditorium>();
            auditorium.Id = auditoriumId;
            _testDataDbSeeder.AddNewAuditoriumToDatabase(auditorium);
            if (!_testDataDbSeeder.CheckingExistingMovieWithImdbId(ImdbId))
            {
                var movie = Fixture.Create<Movie>();
                movie.ImdbId = ImdbId;
                _testDataDbSeeder.AddNewMovieToDataBase(movie);
            }
            
            var request = new CreateShowtimeRequest
            {
                AuditoriumId = auditoriumId,
                ImdbMovieId = ImdbId,
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

        [TestCase(1, "")]
        [TestCase(1, " ")]        
        [TestCase(1, null)]
        [TestCase(-1, "1")]
        [TestCase(0, "1")]
        public async Task CreateShowtimeAsync_ShouldReturn400_WhenCreateShowtimeRequestIsInvalid(int auditoriumId, string movieId)
        {
            //Arrange
            var request = new CreateShowtimeRequest
            {
                AuditoriumId = auditoriumId,
                ImdbMovieId = movieId,
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
            responseErrorMessage.Message.Should().Be("Invalid request.");
            responseErrorMessage.StatusCode.Should().Be((int)result.StatusCode);
        }

        [Test]
        public async Task CreateShowtimeAsync_ShouldReturn400AndReservationId_WhenAuditoriumIdAndMovieIdIsCorrectButSessionDateIsNull()
        {
            //Arrange
            var request = new CreateShowtimeRequest
            {
                AuditoriumId = 1,
                ImdbMovieId = "1",
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
            responseErrorMessage.Message.Should().Be("Invalid request.");
            responseErrorMessage.StatusCode.Should().Be((int)result.StatusCode);
        }
        
        [Test]
        public async Task GetShowtimesAsync_ShouldReturn200_WhenExitingReservationIdExist()
        {
            //Arrange
            var showtimeId = _testDataDbSeeder.AddNewShowtimeToDatabase();

            //Act
            var response = await _client.GetAsync($"/api/showtimes/{showtimeId}");

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Test]
        public async Task GetAllAsync_ShouldReturn200AndListOfShowtimes_WhenSomeShowtimesAreInDb()
        {
            //Arrange
            const int showtimesAmount = 30;
            _testDataDbSeeder.GenerateShowtimesToDatabase(showtimesAmount);

            //Act
            var response = await _client.GetAsync($"/api/showtimes/all");

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var getAllShowtimesResponse = await DeserializeHttpContentAsync<GetAllShowtimesResponse>(response);
            getAllShowtimesResponse.Should().NotBeNull();
            getAllShowtimesResponse.Showtimes.Should().NotBeEmpty();
            getAllShowtimesResponse.Showtimes.Count.Should().Be(showtimesAmount);

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