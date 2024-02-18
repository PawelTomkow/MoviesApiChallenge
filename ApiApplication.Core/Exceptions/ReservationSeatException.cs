using System;
using System.Runtime.Serialization;

namespace ApiApplication.Core.Exceptions
{
    public class ReservationSeatException : Exception
    {
        public ReservationSeatException()
        {
        }

        protected ReservationSeatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ReservationSeatException(string message) : base(message)
        {
        }

        public ReservationSeatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}