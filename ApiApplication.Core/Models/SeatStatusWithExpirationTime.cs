using System;

namespace ApiApplication.Core.Models
{
    public class SeatStatusWithExpirationTime
    {
        public SeatStatus Status { get; set; }
        public bool CanExpired { get; set; }
        public DateTime? Expired { get; set; }
    }
}