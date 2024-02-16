using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiApplication.Clients;
using ApiApplication.Clients.Contracts;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using ApiApplication.Services;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace ApiApplication.Tests.Services
{
    [TestFixture]
    public class MovieServiceTests
    {
        private IMovieService _sut;
        private IMapper _mapper;
        private IApiClient _apiClient;

        [SetUp]
        public void Setup()
        {
            _mapper = Substitute.For<IMapper>();
            _apiClient = Substitute.For<IApiClient>();
            _sut = new MovieService(_mapper, _apiClient);
        }

        [Test]
        public async Task GetAll_ShouldReturnMappedMovies_WhenApiClientReturnsData()
        {
            // Arrange
            var expectedShowResponses = new List<ShowResponse>
            {
                new ShowResponse { Title = "Movie 1" },
                new ShowResponse { Title = "Movie 2" }
            };
            var expectedMovies = expectedShowResponses.Select(sr => new Movie { Title = sr.Title }).ToList();

            _apiClient.GetAllAsync().Returns(Task.FromResult(new ShowListResponse
            {
                ShowResponses = expectedShowResponses
            }));

            _mapper.Map<List<Movie>>(Arg.Any<List<ShowResponse>>()).Returns(expectedMovies);

            // Act
            var result = await _sut.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedMovies);
        }

        [Test]
        public async Task GetAll_ShouldReturnEmptyList_WhenApiClientReturnsNoData()
        {
            // Arrange
            _apiClient.GetAllAsync().Returns(Task.FromResult(new ShowListResponse
            {
                ShowResponses = new List<ShowResponse>()
            }));

            // Act
            var result = await _sut.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}