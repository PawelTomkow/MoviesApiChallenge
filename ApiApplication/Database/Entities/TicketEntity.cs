using System;
using System.Collections.Generic;
using ApiApplication.Core.Services;
using ApiApplication.Services;

namespace ApiApplication.Database.Entities
{
    public class TicketEntity
    {
        public TicketEntity() : this(new DateTimeProvider())
        { }

        public TicketEntity(IDateTimeProvider dateTimeProvider)
        {
            CreatedTime = dateTimeProvider.DateTimeNow;
            Paid = false;
        }

        public Guid Id { get; set; }
        public int ShowtimeId { get; set; }
        public ICollection<SeatEntity> Seats { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Paid { get; set; }
        public ShowtimeEntity Showtime { get; set; }
    }
}
