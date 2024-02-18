using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Core.Exceptions;
using ApiApplication.Core.Models;
using ApiApplication.Core.Services;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ApiApplication.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketsRepository _repository;
        private readonly ILogger<TicketService> _logger;
        private readonly IMapper _mapper;
        private readonly IReservationService _reservationService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IShowtimeService _showtimeService;

        public TicketService(ITicketsRepository repository, 
            ILogger<TicketService> logger, 
            IMapper mapper, 
            IReservationService reservationService,
            IDateTimeProvider dateTimeProvider,
            IShowtimeService showtimeService)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _reservationService = reservationService;
            _dateTimeProvider = dateTimeProvider;
            _showtimeService = showtimeService;
        }
        
        public async Task<Ticket> CreateAsync(string reservationId, CancellationToken cancellationToken)
        {
            var reservation = await _reservationService.GetByIdAsync(reservationId, cancellationToken);
            if (reservation is null)
            {
                throw new ResourceNotFoundException(typeof(Reservation), nameof(reservationId), reservationId);
            }

            var seats = reservation.Seats.Select(x => new SeatEntity
            {
                AuditoriumId = reservation.AuditoriumId,
                SeatNumber = x.SeatNumber,
                Row = x.Row
            });
            
            var ticketEntity = await _repository.CreateAsync(reservation.ShowtimeId, seats, cancellationToken);
            return _mapper.Map<Ticket>(ticketEntity);
        }

        public async Task BuyAsync(string ticketId, CancellationToken cancellationToken)
        {
            Guid.TryParse(ticketId, out var ticketIdGuid);
            var ticket = await _repository.GetAsync(ticketIdGuid, cancellationToken);
            if (ticket is null)
            {
                throw new ResourceNotFoundException(typeof(Ticket), nameof(ticketId), ticketId);
            }

            await _repository.ConfirmPaymentAsync(ticket, cancellationToken);
            _logger.LogInformation("The ticket with id: {id} has been purchased", ticketId);
        }

        public async Task<Ticket> GetAsync(string id, CancellationToken cancellationToken)
        {
            Guid.TryParse(id, out var ticketIdGuid);
            var result = await _repository.GetAsync(ticketIdGuid, cancellationToken);
            if (result is null)
            {
                throw new ResourceNotFoundException(typeof(TicketEntity), nameof(ticketIdGuid), id);
            }

            return _mapper.Map<Ticket>(result);
        }
    }
}