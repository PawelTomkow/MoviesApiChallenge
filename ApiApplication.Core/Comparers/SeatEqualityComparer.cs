using System;
using System.Collections.Generic;
using ApiApplication.Core.Models;

namespace ApiApplication.Core.Comparers
{
    public class SeatEqualityComparer : IEqualityComparer<Seat>
    {
        public bool Equals(Seat x, Seat y)
        {
            // Compare the properties of Seat objects
            return x.Row == y.Row && x.SeatNumber == y.SeatNumber;
        }

        public int GetHashCode(Seat obj)
        {
            // Generate a hash code based on the properties of Seat
            return HashCode.Combine(obj.Row, obj.SeatNumber);
        }
    }
}