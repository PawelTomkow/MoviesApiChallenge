using System.Collections.Generic;
using ApiApplication.Core.Models;

namespace ApiApplication.Controllers.Contracts.Showtimes
{
    public class GetAllShowtimesResponse
    {
        public List<Showtime> Showtimes { get; set; }
    }
}