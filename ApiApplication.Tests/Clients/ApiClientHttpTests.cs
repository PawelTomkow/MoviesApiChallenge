using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ApiApplication.Clients;
using ApiApplication.Clients.Cache;
using ApiApplication.Clients.Contracts;
using ApiApplication.Configurations;
using ApiApplication.Controllers.Contracts;
using ApiApplication.Core.Exceptions;
using AutoFixture;
using FluentAssertions;
using Flurl.Http;
using Flurl.Http.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

namespace ApiApplication.Tests.Clients
{
    [TestFixture]
    public class ApiClientHttpTests
    {
        private ILogger<ApiClientHttp> _logger;
        private ICacheRepository _cacheRepository;
        private IOptions<ApiClientConfiguration> _options;
        private ApiClientHttp _sut;
        private Fixture _fixture;
        private HttpTest _httpTest;

        [SetUp]
        public void Setup()
        {
            _httpTest = new HttpTest();
            Substitute.For<IFlurlClient>();
            _logger = Substitute.For<ILogger<ApiClientHttp>>();
            _cacheRepository = Substitute.For<ICacheRepository>();
            var apiClientConfiguration = new ApiClientConfiguration()
            {
                BaseAddress = "http://baseaddres",
                ApiKey = "ApiKey"
            };
            _options = Substitute.For<IOptions<ApiClientConfiguration>>();
            _options.Value.Returns(apiClientConfiguration);
            _fixture = new Fixture();
            _sut = new ApiClientHttp(_logger, _cacheRepository, _options);
        }
        
        [TearDown]
        public void DisposeHttpTest() {
            _httpTest.Dispose();
        }

        #region GetByIdAsync

        [Test]
        public async Task GetByIdAsync_ShouldReturnShowResponse_WhenAllDataAndResponseAreCorrect()
        {
            //Arrange
            var expectedShowResponse = _fixture.Create<ShowResponse>();
            _httpTest
                .RespondWithJson(expectedShowResponse, status: (int)HttpStatusCode.OK);
            
            //Act
            var result = await _sut.GetByIdAsync("123");

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedShowResponse);
        }
        
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public async Task GetByIdAsync_ShouldThrowArgumentException_WhenIdIsIncorrect(string id)
        {
            //Arrange
            var expectedShowResponse = _fixture.Create<ShowResponse>();
            _httpTest
                .RespondWithJson(expectedShowResponse, status: (int)HttpStatusCode.OK);
            
            //Act
            Func<Task> act = async () => await _sut.GetByIdAsync(id);

            //Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [TestCase(401)]
        [TestCase(404)]
        public async Task GetByIdAsync_ShouldReturnNull_WhenStatusCodeIs(int statusCode)
        {
            //Arrange
            var expectedErrorResponse = _fixture.Create<ErrorResponse>();
            _httpTest
                .RespondWithJson(expectedErrorResponse, status: statusCode);
            
            //Act
            var result = await _sut.GetByIdAsync("123");

            //Assert
            result.Should().BeNull();
        }
        
        [TestCase(500)]
        [TestCase(418)]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotSupportedStatusCodeIs(int statusCode)
        {
            //Arrange
            var expectedErrorResponse = _fixture.Create<ErrorResponse>();
            _httpTest
                .RespondWithJson(expectedErrorResponse, status: statusCode);
            
            //Act
            var result = await _sut.GetByIdAsync("123");

            //Assert
            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenFlurlIsThrowingFlurlHttpTimeoutException()
        {
            //Arrange
            _httpTest
                .SimulateTimeout();
            
            //Act
            var result = await _sut.GetByIdAsync("123");

            //Assert
            result.Should().BeNull();
        }
        
        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenFlurlIsThrowingUnknownException()
        {
            //Arrange
            var unknownException = Substitute.For<UnknownErrorException>();
            _httpTest
                .SimulateException(unknownException);
            
            //Act
            var result = await _sut.GetByIdAsync("123");

            //Assert
            result.Should().BeNull();
        }

        #endregion

        #region SearchAsync

        [Test]
        public async Task SearchAsync_ShouldReturnShowResponse_WhenAllDataAndResponseAreCorrect()
        {
            //Arrange
            var expectedShowResponse = _fixture.Create<List<ShowResponse>>();
            _httpTest
                .RespondWithJson(expectedShowResponse, status: (int)HttpStatusCode.OK);
            
            //Act
            var result = await _sut.SearchAsync("123");

            //Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().NotBeEmpty();
            result.ShowResponses.Should().BeEquivalentTo(expectedShowResponse);
        }
        
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public async Task SearchAsync_ShouldThrowArgumentException_WhenTextIsIncorrect(string text)
        {
            //Arrange
            var expectedShowResponse = _fixture.Create<ShowListResponse>();
            _httpTest
                .RespondWithJson(expectedShowResponse, status: (int)HttpStatusCode.OK);
            
            //Act
            Func<Task> act = async () => await _sut.SearchAsync(text);

            //Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [TestCase(401)]
        [TestCase(404)]
        public async Task SearchAsync_ShouldReturnShowListResponseWithEmptyList_WhenStatusCodeIs(int statusCode)
        {
            //Arrange
            var expectedErrorResponse = _fixture.Create<ErrorResponse>();
            _httpTest
                .RespondWithJson(expectedErrorResponse, status: statusCode);
            
            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().BeEmpty();
        }
        
        [TestCase(500)]
        [TestCase(418)]
        public async Task SearchAsync_ShouldReturnShowListResponseWithEmptyList_WhenNotSupportedStatusCodeIs(int statusCode)
        {
            //Arrange
            var expectedErrorResponse = _fixture.Create<ErrorResponse>();
            _httpTest
                .RespondWithJson(expectedErrorResponse, status: statusCode);
            
            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().BeEmpty();
        }
        
        [Test]
        public async Task SearchAsync_ShouldReturnShowListResponseWithEmptyList_WhenFlurlIsThrowingFlurlHttpTimeoutException()
        {
            //Arrange
            _httpTest
                .SimulateTimeout();
            
            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().BeEmpty();
        }
        
        [Test]
        public async Task SearchAsync_ShouldReturnShowListResponseWithEmptyList_WhenFlurlIsThrowingUnknownException()
        {
            //Arrange
            var unknownException = Substitute.For<UnknownErrorException>();
            _httpTest
                .SimulateException(unknownException);
            
            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().BeEmpty();
        }

        #endregion

        #region GetAllAsync

        [Test]
        public async Task GetAllAsync_ShouldReturnShowResponse_WhenAllDataAndResponseAreCorrect()
        {
            //Arrange
            var expectedShowResponse = _fixture.Create<List<ShowResponse>>();
            _httpTest
                .RespondWithJson(expectedShowResponse, status: (int)HttpStatusCode.OK);
            
            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().NotBeEmpty();
            result.ShowResponses.Should().BeEquivalentTo(expectedShowResponse);
        }

        [TestCase(401)]
        [TestCase(404)]
        public async Task GetAllAsync_ShouldReturnShowListResponseWithEmptyList_WhenStatusCodeIs(int statusCode)
        {
            //Arrange
            var expectedErrorResponse = _fixture.Create<ErrorResponse>();
            _httpTest
                .RespondWithJson(expectedErrorResponse, status: statusCode);
            
            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().BeEmpty();
        }
        
        [TestCase(500)]
        [TestCase(418)]
        public async Task GetAllAsync_ShouldReturnShowListResponseWithEmptyList_WhenNotSupportedStatusCodeIs(int statusCode)
        {
            //Arrange
            var expectedErrorResponse = _fixture.Create<ErrorResponse>();
            _httpTest
                .RespondWithJson(expectedErrorResponse, status: statusCode);
            
            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().BeEmpty();
        }
        
        [Test]
        public async Task GetAllAsync_ShouldReturnShowListResponseWithEmptyList_WhenFlurlIsThrowingFlurlHttpTimeoutException()
        {
            //Arrange
            _httpTest
                .SimulateTimeout();
            
            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().BeEmpty();
        }
        
        [Test]
        public async Task GetAllAsync_ShouldReturnShowListResponseWithEmptyList_WhenFlurlIsThrowingUnknownException()
        {
            //Arrange
            var unknownException = Substitute.For<UnknownErrorException>();
            _httpTest
                .SimulateException(unknownException);
            
            //Act
            var result = await _sut.GetAllAsync();

            //Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().BeEmpty();
        }

        #endregion
    }
}