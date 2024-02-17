using System;
using System.Collections.Generic;
using ApiApplication.Core.Services;

namespace ApiApplication.Core.Models
{
    public class Ticket
    {
        public Ticket() : this(new DateTimeProvider())
        { }

        public Ticket(IDateTimeProvider dateTimeProvider)
        {
            CreatedTime = dateTimeProvider.DateTimeNow;
            Paid = false;
        }

        public Guid Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Paid { get; set; }
        public List<Seat> Seats { get; set; }
    }
}