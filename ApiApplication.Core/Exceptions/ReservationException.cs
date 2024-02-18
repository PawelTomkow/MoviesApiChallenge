using System;
using System.Runtime.Serialization;

namespace ApiApplication.Core.Exceptions
{
    public class ReservationException : Exception
    {
        public ReservationException()
        {
        }

        protected ReservationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ReservationException(string message) : base(message)
        {
        }

        public ReservationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}