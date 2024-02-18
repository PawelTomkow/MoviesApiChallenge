using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Clients.Cache;
using ApiApplication.Configurations;
using ApiApplication.Core.Exceptions;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using ApiApplication.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;

namespace ApiApplication.Tests.Services
{
    [TestFixture]
    public class SeatServiceTests
    {
        private IShowtimeService _showtimeService;
        private IAuditoriumService _auditoriumService;
        private ILogger<SeatService> _logger;
        private ICacheRepository _cache;
        private SeatService _sut;

        [SetUp]
        public void Setup()
        {
            _showtimeService = Substitute.For<IShowtimeService>();
            _auditoriumService = Substitute.For<IAuditoriumService>();
            _logger = Substitute.For<ILogger<SeatService>>();
            _cache = Substitute.For<ICacheRepository>();
            var reservationConfig = new ReservationConfiguration
            {
                ExpiryTime = 5
            };
            var options = Substitute.For<IOptions<ReservationConfiguration>>();
            options.Value.Returns(reservationConfig);
            _sut = new SeatService(_auditoriumService, 
                _showtimeService,
                _logger,
                _cache, 
                options, 
                new DateTimeProvider());
        }
        
        [Test]
        public void GetWithStatusByShowtimeId_ShouldThrowException_WhenSoldSeatsNotFound()
        {
            // Arrange
            const int showtimeId = 123;
            var cancellationToken = CancellationToken.None;

            SoldSeats soldSeats = null;
            _showtimeService.GetByIdWithAuditoriumIdAsync(showtimeId, cancellationToken).Returns(soldSeats);

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(async () =>
            {
                await _sut.GetWithStatusByShowtimeIdAsync(showtimeId, cancellationToken);
            });
        }
        
        [Test]
        public async Task GetStatusAsync_ShouldReturnCachedStatus_WhenAvailable()
        {
            // Arrange
            const int showtimeId = 123;
            const short seatNumber = 1;
            const short row = 1;
            var cancellationToken = CancellationToken.None;

            _cache.GetValueAsync<SeatStatusWithExpirationTime>(Arg.Any<string>()).Returns(new SeatStatusWithExpirationTime()
            {
                Status = SeatStatus.Sold,
                CanExpired = false
            });

            // Act
            var result = await _sut.GetStatusAsync(showtimeId, seatNumber, row, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(SeatStatus.Sold);
        }
        
        [Test]
        public async Task GetSeatsByAuditoriumIdAsync_Should_Return_Seats_When_Auditorium_Exists()
        {
            // Arrange
            const int auditoriumId = 1;
            var cancellationToken = CancellationToken.None;

            var expectedSeats = new List<Seat>
            {
                new Seat { SeatNumber = 1, Row = 1 },
                new Seat { SeatNumber = 1, Row = 2 },
                new Seat { SeatNumber = 1, Row = 3 },
            };
            _auditoriumService.GetByIdAsync(auditoriumId, cancellationToken)
                .Returns(new Auditorium { Id = auditoriumId, Seats = expectedSeats });
            

            // Act
            var result = await _sut.GetSeatsByAuditoriumIdAsync(auditoriumId, cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedSeats);
        }

        [Test]
        public void GetSeatsByAuditoriumIdAsync_Should_Throw_Exception_When_Auditorium_Does_Not_Exist()
        {
            // Arrange
            const int auditoriumId = 1;
            var cancellationToken = CancellationToken.None;

            _auditoriumService.GetByIdAsync(auditoriumId, cancellationToken).Returns((Auditorium)null);

            // Act & Assert
            Assert.ThrowsAsync<ResourceNotFoundException>(async () =>
            {
                await _sut.GetSeatsByAuditoriumIdAsync(auditoriumId, cancellationToken);
            });
        }
    }
}