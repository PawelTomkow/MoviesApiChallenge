using System.Collections.Generic;
using ApiApplication.Core.Models;

namespace ApiApplication.Controllers.Contracts.Reservations
{
    public class CreateReservationRequest
    {
        public string ShowtimeId { get; set; }
        public List<Seat> Seats { get; set; }
        public int AuditoriumId { get; set; }
    }
}