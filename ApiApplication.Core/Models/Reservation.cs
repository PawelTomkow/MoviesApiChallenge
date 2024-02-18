using System.Collections.Generic;

namespace ApiApplication.Core.Models
{
    public class Reservation
    {
        public string Id { get; set; }
        public int AuditoriumId { get; set; }
        public int ShowtimeId { get; set; }
        public List<Seat> Seats { get; set; }
        public Movie Movie { get; set; }
    }
}