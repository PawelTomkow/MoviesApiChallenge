﻿using System.Collections.Generic;
using ApiApplication.Core.Models;

namespace ApiApplication.Controllers.Contracts.Seats
{
    public class GetShowtimeSeatsWithStatusResponse
    {
        public List<SeatWithStatus> SeatsWithStatus { get; set; }
    }
}