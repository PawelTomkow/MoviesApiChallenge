using System.Collections.Generic;

namespace ApiApplication.Controllers.Contracts.Reservations
{
    public class CreateReservationRequest
    {
        public string ShowtimeId { get; set; }
        public List<int> Seats { get; set; }
        public int AuditoriumId { get; set; }
    }
}