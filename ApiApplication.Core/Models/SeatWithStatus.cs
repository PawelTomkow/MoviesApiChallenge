namespace ApiApplication.Core.Models
{
    public class SeatWithStatus
    {
        public SeatStatus Status { get; set; }
        public Seat Seat { get; set; }
    }
}