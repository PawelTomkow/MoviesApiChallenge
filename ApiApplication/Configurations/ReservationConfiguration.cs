namespace ApiApplication.Configurations
{
    public class ReservationConfiguration
    {
        public const string Name = "Reservation";
        
        /// <summary>
        /// Time in s.
        /// </summary>
        public int ExpiryTime { get; set; }
    }
}