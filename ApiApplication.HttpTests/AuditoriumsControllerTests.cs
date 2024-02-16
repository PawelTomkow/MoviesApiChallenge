using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Controllers.Contracts.Auditoriums;
using ApiApplication.Core.Models;
using ApiApplication.HttpTests.Base;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace ApiApplication.HttpTests
{
    public class AuditoriumsControllerTests : HttpRequestCreator
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
        public async Task GetAllAsync_ShouldReturn200AndGetAllAuditoriumsResponse()
        {
            //Act
            var response = await _client.GetAsync("api/auditorium/all");

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseObj = await DeserializeHttpContentAsync<GetAllAuditoriumsResponse>(response);
            responseObj.Auditoriums.Should().NotBeNull().And.NotBeEmpty();
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturn200AndAuditorium_WhenIdExist()
        {
            //Arrange
            var auditorium = Fixture.Create<Auditorium>();
            auditorium.Id = 0;
            var addedAuditoriumId = _testDataDbSeeder.AddNewAuditoriumToDatabase(auditorium);

            //Act
            var result = await _client.GetAsync($"api/auditorium/{addedAuditoriumId}");

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultObj = await DeserializeHttpContentAsync<Auditorium>(result);
            resultObj.Should().NotBeNull();
            resultObj.Id.Should().Be(addedAuditoriumId);
        }

        [TestCase(-1)]
        [TestCase(0)]
        public async Task GetByIdAsync_ShouldReturn400_WhenIdIsIncorrect(int auditoriumId)
        {
            //Act
            var result = await _client.GetAsync($"api/auditorium/{auditoriumId}");

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var resultObj = await DeserializeHttpContentAsync<ErrorResponse>(result);
            resultObj.Should().NotBeNull();
            resultObj.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            resultObj.Message.Should().Be("Invalid request.");
        }
    }
}