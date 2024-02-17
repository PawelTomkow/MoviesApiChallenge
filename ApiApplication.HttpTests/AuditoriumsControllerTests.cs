using System.Collections.Generic;
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
            //Arrange
            var auditorium = Fixture.Create<List<Auditorium>>();
            _testDataDbSeeder.AddNewAuditoriumsToDatabase(auditorium);
            
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
            var auditoriumFromDb = _testDataDbSeeder.AddNewAuditoriumToDatabase(auditorium);

            //Act
            var result = await _client.GetAsync($"api/auditorium/{auditoriumFromDb}");

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultObj = await DeserializeHttpContentAsync<Auditorium>(result);
            resultObj.Should().NotBeNull();
            resultObj.Id.Should().Be(auditoriumFromDb);
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
        
        [Test]
        public async Task GetByIdAsync_ShouldReturn404_WhenNotExist()
        {
            //Arrange
            const int notExistedAuditoriumId = 99999999;

            //Act
            var result = await _client.GetAsync($"api/auditorium/{notExistedAuditoriumId}");

            //Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var resultObj = await DeserializeHttpContentAsync<ErrorResponse>(result);
            resultObj.Should().NotBeNull();
            resultObj.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            resultObj.Message.Should().Be($"Auditorium with id: {notExistedAuditoriumId} not found.");
        }
    }
}