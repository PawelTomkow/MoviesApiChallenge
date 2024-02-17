using System.Collections.Generic;

namespace ApiApplication.Core.Models
{
    public class Showtime
    {
        public int Id { get; set; }
        public Movie Movie { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}