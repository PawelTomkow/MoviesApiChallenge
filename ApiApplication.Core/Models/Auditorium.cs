using System.Collections.Generic;

namespace ApiApplication.Core.Models
{
    public class Auditorium
    {
        public int Id { get; set; }
        public List<Showtime> Showtimes { get; set; }
        public List<Seat> Seats { get; set; }
    }
}