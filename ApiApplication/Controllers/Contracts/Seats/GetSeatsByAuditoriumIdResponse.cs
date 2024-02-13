using System.Collections.Generic;
using ApiApplication.Core.Models;

namespace ApiApplication.Controllers.Contracts.Seats
{
    public class GetSeatsByAuditoriumIdResponse
    {
        public int AuditoriumId { get; set; }
        public List<Seat> Seats { get; set; }
        public int TotalSeats => Seats?.Count ?? 0;
    }
}