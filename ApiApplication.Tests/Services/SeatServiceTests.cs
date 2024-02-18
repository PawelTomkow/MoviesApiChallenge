using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Clients.Cache;
using ApiApplication.Core.Exceptions;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using ApiApplication.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace ApiApplication.Tests.Services
{
    [TestFixture]
    public class SeatServiceTests
    {
        private IShowtimeService _showtimeService;
        private IAuditoriumService _auditoriumService;
        private IReservationService _reservationService;
        private ILogger<SeatService> _logger;
        private ICacheRepository _cache;
        private SeatService _sut;

        [SetUp]
        public void Setup()
        {
            _showtimeService = Substitute.For<IShowtimeService>();
            _auditoriumService = Substitute.For<IAuditoriumService>();
            _logger = Substitute.For<ILogger<SeatService>>();
            _reservationService = Substitute.For<IReservationService>();
            _cache = Substitute.For<ICacheRepository>();
            _sut = new SeatService(_auditoriumService, _showtimeService,_logger, _reservationService, _cache);
        }

        [Test]
        public async Task GetWithStatusByShowtimeId_ShouldReturnSeatsWithStatus()
        {
            // Arrange
            const int showtimeId = 123;
            var cancellationToken = CancellationToken.None;

            var soldSeats = new SoldSeats
            {
                AuditoriumId = 1,
                Seats = new List<Seat> { new Seat { Row = 1, SeatNumber = 1 } }
            };
            _showtimeService.GetByIdWithAuditoriumIdAsync(showtimeId, cancellationToken).Returns(soldSeats);

            var auditorium = new Auditorium
            {
                Seats = new List<Seat> { new Seat { Row = 1, SeatNumber = 1 }, new Seat { Row = 1, SeatNumber = 2 }, new Seat { Row = 1, SeatNumber = 3 } }
            };
            _auditoriumService.GetByIdAsync(soldSeats.AuditoriumId, cancellationToken).Returns(auditorium);

            var reservedSeats = new List<Seat> { new Seat { Row = 1, SeatNumber = 2 } };
            _reservationService.GetReservedSeatsByShowtimeId(showtimeId, cancellationToken).Returns(reservedSeats);

            // Act
            var result = await _sut.GetWithStatusByShowtimeIdAsync(showtimeId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result[0].Seat.Should().BeEquivalentTo(new Seat { Row = 1, SeatNumber = 1 });
            result[1].Status.Should().Be(SeatStatus.Reserved);
        }
        
        [Test]
        public async Task GetWithStatusByShowtimeId_ShouldReturnSeatsWithAllFreeStatus_WhenNoSoldOrReservedSeats()
        {
            // Arrange
            const int showtimeId = 123;
            var cancellationToken = CancellationToken.None;

            var soldSeats = new SoldSeats
            {
                AuditoriumId = 1,
                Seats = new List<Seat>()
            };
            _showtimeService.GetByIdWithAuditoriumIdAsync(showtimeId, cancellationToken).Returns(soldSeats);

            var auditorium = new Auditorium
            {
                Seats = new List<Seat> { new Seat { Row = 1, SeatNumber = 1 }, new Seat { Row = 1, SeatNumber = 2 } }
            };
            _auditoriumService.GetByIdAsync(soldSeats.AuditoriumId, cancellationToken).Returns(auditorium);

            var reservedSeats = new List<Seat>();
            _reservationService.GetReservedSeatsByShowtimeId(showtimeId, cancellationToken).Returns(reservedSeats);

            // Act
            var result = await _sut.GetWithStatusByShowtimeIdAsync(showtimeId, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().OnlyContain(seat => seat.Status == SeatStatus.Free);
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

            _cache.GetValueAsync<SeatStatus>(Arg.Any<string>()).Returns(SeatStatus.Reserved);

            // Act
            var result = await _sut.GetStatusAsync(showtimeId, seatNumber, row, cancellationToken);

            // Assert
            result.Should().Be(SeatStatus.Reserved);
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