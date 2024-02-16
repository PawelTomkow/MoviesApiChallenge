using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Seats;
using ApiApplication.Core.Models;
using ApiApplication.HttpTests.Base;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace ApiApplication.HttpTests
{
    public class SeatsControllerTests : HttpRequestCreator
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
            _client.Dispose();
            _server.Dispose();
        }

        [Test]
        public async Task
            GetSeatsByAuditoriumIdAsync_ShouldReturn200AndGetSeatsByAuditoriumIdResponse_WhenAuditoriumIdExist()
        {
            //Arrange
            var auditorium = new Auditorium();
            var addedAuditoriumId = _testDataDbSeeder.AddNewAuditoriumToDatabase(auditorium);
            
            //Act
            var result = await _client.GetAsync($"api/seats/{addedAuditoriumId}");

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var resultObj = await DeserializeHttpContentAsync<GetSeatsByAuditoriumIdResponse>(result);
            resultObj.Seats.Should().NotBeNull().And.NotBeEmpty();
        }

        [Test]
        public async Task GetSeatsByAuditoriumIdAsync_ShouldReturn400_WhenAuditoriumIdIsIncorrect()
        {
            //Arrange
            const int notExistAuditoriumId = -1;
            
            //Act
            var result = await _client.GetAsync($"api/seats/{notExistAuditoriumId}");

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var resultObj = await DeserializeHttpContentAsync<ErrorResponse>(result);
            resultObj.Message.Should().BeEmpty()
                .And.Be("Invalid request.");
        }

        [Test]
        public async Task GetShowtimeSeatsWithStatus_ShouldReturn200AndSeatsWithStatus_WhenShowtimeExistAndHaveSeats()
        {
            //Arrange
            var auditorium = Fixture.Create<Auditorium>();
            var addedAuditoriumId = _testDataDbSeeder.AddNewAuditoriumToDatabase(auditorium);
            var showtime = Fixture.Create<Showtime>();
            showtime.AuditoriumId = addedAuditoriumId;
            var addedShowtimeId = _testDataDbSeeder.AddNewShowtimeToDatabase(showtime);

            //Act
            var result = await _client.GetAsync($"api/seats/status/{addedShowtimeId}");

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var resultObj = await DeserializeHttpContentAsync<GetShowtimeSeatsWithStatusResponse>(result);
            resultObj.Should().NotBeNull();
            resultObj.SeatsWithStatus.Should().NotBeNull().And.NotBeEmpty();
        }

        [Test]
        public async Task GetShowtimeSeatsWithStatus_ShouldReturn400_WhenShowtimeIsIncorrect()
        {
            //Arrange
            const int incorrectShowtimeId = -1;
            
            //Act
            var result = await _client.GetAsync($"api/seats/status/{incorrectShowtimeId}");

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            
            var responseErrorMessage = await DeserializeHttpContentAsync<ErrorResponse>(result);
            responseErrorMessage.Should().NotBeNull();
            responseErrorMessage.Message.Should().Be("Invalid request.");
            responseErrorMessage.StatusCode.Should().Be((int)result.StatusCode);
        }
    }
}