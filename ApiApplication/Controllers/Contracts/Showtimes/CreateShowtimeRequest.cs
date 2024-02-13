using System;

namespace ApiApplication.Controllers.Contracts.Showtimes
{
    public class CreateShowtimeRequest
    {
        public string MovieId { get; set; }
        public DateTime? SessionDate { get; set; }
        public int AuditoriumId { get; set; }
    }
}