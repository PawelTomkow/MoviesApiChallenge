using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ApiApplication.Clients;
using ApiApplication.Clients.Cache;
using ApiApplication.Clients.Contracts;
using ApiApplication.Configurations;
using ApiApplication.Configurations.Mapper;
using ApiApplication.Core.Exceptions;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using ProtoDefinitions;

namespace ApiApplication.Tests
{
    [TestFixture]
    public class ApiClientGrpcTests
    {
        private IMapper _mapper;
        private IOptions<ApiClientConfiguration> _apiClientOptions;
        private MoviesApi.MoviesApiClient _moviesApiClient;
        private ApiClientGrpc _apiClient;
        private Fixture _fixture;
        private ILogger<ApiClientGrpc> _logger;
        private ICacheRepository _cache;

        [SetUp]
        public void Setup()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ShowResponseProfile>();
                cfg.AddProfile<ShowListResponseProfile>();
            });
            _mapper = mapperConfig.CreateMapper();
            _apiClientOptions = Options.Create(new ApiClientConfiguration
            {
                ApiKey = "your_api_key",
                BaseAddress = "http://example.com"
            });
            _logger = Substitute.For<ILogger<ApiClientGrpc>>();
            _moviesApiClient = Substitute.For<MoviesApi.MoviesApiClient>();
            _cache = Substitute.For<ICacheRepository>();
            _apiClient = new ApiClientGrpc(_apiClientOptions, _mapper, _moviesApiClient, _cache, _logger);
            _fixture = new Fixture();
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnSuccessfulResponse_ReturnsMappedData()
        {
            // Arrange
            const string id = "123";
            var expectedResponse = _fixture.Create<showResponse>();
            var responseModel = _fixture.Create<responseModel>();
            responseModel.Data = Google.Protobuf.WellKnownTypes.Any.Pack(expectedResponse);
            responseModel.Success = true;
            var call = TestCalls.AsyncUnaryCall(Task.FromResult(responseModel), Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess, () => new Metadata(), () => { });
            _moviesApiClient.GetByIdAsync(Arg.Any<IdRequest>()).Returns(call);

            // Act
            var result = await _apiClient.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedResponse.Id);
            result.Rank.Should().Be(expectedResponse.Rank);
            result.Title.Should().Be(expectedResponse.Title);
            result.FullTitle.Should().Be(expectedResponse.FullTitle);
            result.Year.Should().Be(expectedResponse.Year);
            result.Image.Should().Be(expectedResponse.Image);
            result.Crew.Should().Be(expectedResponse.Crew);
            result.ImDbRating.Should().Be(expectedResponse.ImDbRating);
            result.ImDbRatingCount.Should().Be(expectedResponse.ImDbRatingCount);
        }
        
        [Test]
        public async Task GetByIdAsync_ShouldReturnSuccessfulResponse_ReturnsShowResponseFromCacheWhenGrpcNotWorking()
        {
            // Arrange
            const string id = "123";
            var expectedResponse = _fixture.Create<ShowResponse>();
            _moviesApiClient.GetByIdAsync(Arg.Any<IdRequest>()).Throws(new RpcException(new Status(StatusCode.Cancelled, "detail")));
            _cache.GetValueAsync<ShowResponse>(Arg.Any<string>()).Returns(expectedResponse);
            
            // Act
            var result = await _apiClient.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedResponse.Id);
            result.Rank.Should().Be(expectedResponse.Rank);
            result.Title.Should().Be(expectedResponse.Title);
            result.FullTitle.Should().Be(expectedResponse.FullTitle);
            result.Year.Should().Be(expectedResponse.Year);
            result.Image.Should().Be(expectedResponse.Image);
            result.Crew.Should().Be(expectedResponse.Crew);
            result.ImDbRating.Should().Be(expectedResponse.ImDbRating);
            result.ImDbRatingCount.Should().Be(expectedResponse.ImDbRatingCount);
        }
        
        [Test]
        public async Task GetByIdAsync_ShouldThrowResourceUnavailableException_WhenGrpcAndCacheUnavailable()
        {
            // Arrange
            const string id = "123";
            var expectedResponse = _fixture.Create<ShowResponse>();
            _moviesApiClient.GetByIdAsync(Arg.Any<IdRequest>()).Throws(new RpcException(new Status(StatusCode.Cancelled, "detail")));
            _cache.GetValueAsync<ShowResponse>(Arg.Any<string>()).Returns(default(ShowResponse));
            
            // Act
            Func<Task> act = async () => await _apiClient.GetByIdAsync(id);

            // Assert
            await act.Should().ThrowAsync<ResourceUnavailableException>();
        }
        
        [Test]
        public async Task GetByIdAsync_ShouldThrowResourceUnavailableException_WhenResponseItIsNotPossibleToGet()
        {
            // Arrange
            const string id = "123";
            var responseModel = _fixture.Create<responseModel>();
            responseModel.Success = false;
            var call = TestCalls.AsyncUnaryCall(Task.FromResult(responseModel), Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess, () => new Metadata(), () => { });
            _moviesApiClient.GetByIdAsync(Arg.Any<IdRequest>()).Returns(call);

            // Act
            Func<Task> act = async () => await _apiClient.GetByIdAsync(id);

            // Assert
            await act.Should().ThrowAsync<ResourceUnavailableException>();
        }
        
        [Test]
        public async Task SearchAsync_ShouldReturnSuccessfulResponse_ReturnsMappedData()
        {
            // Arrange
            const string id = "123";
            var expectedShowResponse = _fixture.Create<showResponse>();
            var expectedShowListResponse = _fixture.Create<showListResponse>();
            SetPrivateField(expectedShowListResponse, "shows_", new RepeatedField<showResponse>() { expectedShowResponse });
            var responseModel = _fixture.Create<responseModel>();
            responseModel.Data = Google.Protobuf.WellKnownTypes.Any.Pack(expectedShowListResponse);
            responseModel.Success = true;
            var call = TestCalls.AsyncUnaryCall(Task.FromResult(responseModel), Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess, () => new Metadata(), () => { });
            _moviesApiClient.SearchAsync(Arg.Any<SearchRequest>()).Returns(call);

            // Act
            var result = await _apiClient.SearchAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().NotBeEmpty();
            var firstElementResponse = result.ShowResponses.FirstOrDefault();
            firstElementResponse.Should().NotBeNull();
            firstElementResponse.Id.Should().Be(expectedShowResponse.Id);
            firstElementResponse.Rank.Should().Be(expectedShowResponse.Rank);
            firstElementResponse.Title.Should().Be(expectedShowResponse.Title);
            firstElementResponse.FullTitle.Should().Be(expectedShowResponse.FullTitle);
            firstElementResponse.Year.Should().Be(expectedShowResponse.Year);
            firstElementResponse.Image.Should().Be(expectedShowResponse.Image);
            firstElementResponse.Crew.Should().Be(expectedShowResponse.Crew);
            firstElementResponse.ImDbRating.Should().Be(expectedShowResponse.ImDbRating);
            firstElementResponse.ImDbRatingCount.Should().Be(expectedShowResponse.ImDbRatingCount);
        }
        
        [Test]
        public async Task SearchAsync_ShouldThrowResourceUnavailableException_WhenResponseItIsNotPossibleToGet()
        {
            // Arrange
            const string id = "123";
            var responseModel = _fixture.Create<responseModel>();
            responseModel.Success = false;
            var call = TestCalls.AsyncUnaryCall(Task.FromResult(responseModel), Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess, () => new Metadata(), () => { });
            _moviesApiClient.SearchAsync(Arg.Any<SearchRequest>()).Returns(call);

            // Act
            Func<Task> act = async () => await _apiClient.SearchAsync(id);

            // Assert
            await act.Should().ThrowAsync<ResourceUnavailableException>();
        }
        
        [Test]
        public async Task SearchAsync_ShouldThrowResourceUnavailableException_WhenGrpcAndCacheUnavailable()
        {
            // Arrange
            const string id = "123";
            var expectedResponse = _fixture.Create<ShowListResponse>();
            _moviesApiClient.SearchAsync(Arg.Any<SearchRequest>()).Throws(new RpcException(new Status(StatusCode.Cancelled, "detail")));
            _cache.GetValueAsync<ShowListResponse>(Arg.Any<string>()).Returns(default(ShowListResponse));
            
            // Act
            Func<Task> act = async () => await _apiClient.GetByIdAsync(id);

            // Assert
            await act.Should().ThrowAsync<ResourceUnavailableException>();
        }
        
        [Test]
        public async Task GetAllAsync_ShouldReturnSuccessfulResponse_ReturnsMappedData()
        {
            // Arrange
            var expectedShowResponse = _fixture.Create<showResponse>();
            var expectedShowListResponse = _fixture.Create<showListResponse>();
            SetPrivateField(expectedShowListResponse, "shows_", new RepeatedField<showResponse>() { expectedShowResponse });
            var responseModel = _fixture.Create<responseModel>();
            responseModel.Data = Google.Protobuf.WellKnownTypes.Any.Pack(expectedShowListResponse);
            responseModel.Success = true;
            var call = TestCalls.AsyncUnaryCall(Task.FromResult(responseModel), Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess, () => new Metadata(), () => { });
            _moviesApiClient.GetAllAsync(Arg.Any<Empty>()).Returns(call);

            // Act
            var result = await _apiClient.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.ShowResponses.Should().NotBeEmpty();
            var firstElementResponse = result.ShowResponses.FirstOrDefault();
            firstElementResponse.Should().NotBeNull();
            firstElementResponse.Id.Should().Be(expectedShowResponse.Id);
            firstElementResponse.Rank.Should().Be(expectedShowResponse.Rank);
            firstElementResponse.Title.Should().Be(expectedShowResponse.Title);
            firstElementResponse.FullTitle.Should().Be(expectedShowResponse.FullTitle);
            firstElementResponse.Year.Should().Be(expectedShowResponse.Year);
            firstElementResponse.Image.Should().Be(expectedShowResponse.Image);
            firstElementResponse.Crew.Should().Be(expectedShowResponse.Crew);
            firstElementResponse.ImDbRating.Should().Be(expectedShowResponse.ImDbRating);
            firstElementResponse.ImDbRatingCount.Should().Be(expectedShowResponse.ImDbRatingCount);
        }

        [Test]
        public async Task GetAllAsync_ShouldThrowResourceUnavailableException_WhenResponseItIsNotPossibleToGet()
        {
            // Arrange
            var responseModel = _fixture.Create<responseModel>();
            responseModel.Success = false;
            var call = TestCalls.AsyncUnaryCall(Task.FromResult(responseModel), Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess, () => new Metadata(), () => { });
            _moviesApiClient.GetAllAsync(Arg.Any<Empty>()).Returns(call);

            // Act
            Func<Task> act = async () => await _apiClient.GetAllAsync();

            // Assert
            await act.Should().ThrowAsync<ResourceUnavailableException>();
        }
        
        [Test]
        public async Task GetAllAsync_ShouldThrowResourceUnavailableException_WhenGrpcAndCacheUnavailable()
        {
            // Arrange
            const string id = "123";
            var expectedResponse = _fixture.Create<ShowListResponse>();
            _moviesApiClient.GetAllAsync(Arg.Any<Empty>()).Throws(new RpcException(new Status(StatusCode.Cancelled, "detail")));
            _cache.GetValueAsync<ShowListResponse>(Arg.Any<string>()).Returns(default(ShowListResponse));
            
            // Act
            Func<Task> act = async () => await _apiClient.GetByIdAsync(id);

            // Assert
            await act.Should().ThrowAsync<ResourceUnavailableException>();
        }

        private void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null) field.SetValue(obj, value);
        }
        
        // Write similar tests for other methods...
    }
}