using System.Collections.Generic;

namespace ApiApplication.Core.Models
{
    public class SoldSeats
    {
        public Showtime Showtime { get; set; }
        public int AuditoriumId { get; set; }
        public List<Seat> Seats { get; set; }
    }
}