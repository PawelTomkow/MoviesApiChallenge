using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApiApplication.Controllers.Contracts.Repertoires;
using ApiApplication.Core.Models;
using ApiApplication.HttpTests.Base;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace ApiApplication.HttpTests
{
    public class MoviesControllerTests : HttpRequestCreator
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
        public async Task GetRepertoiresAsync_ShouldReturn200AndListOfRepertoires()
        {
            //Arrange
            
            //Act
            var response = await _client.GetAsync("api/movies/all");

            //Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseObject = await DeserializeHttpContentAsync<PossibleRepertoiresResponse>(response);
            responseObject.Repertoires.Should().BeOfType<IEnumerable<Movie>>();
        }
    }
}