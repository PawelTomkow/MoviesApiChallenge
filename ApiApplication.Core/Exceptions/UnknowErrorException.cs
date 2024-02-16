using System;
using System.Runtime.Serialization;

namespace ApiApplication.Core.Exceptions
{
    public class UnknownErrorException : Exception
    {
        public UnknownErrorException()
        {
        }

        protected UnknownErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UnknownErrorException(string message) : base(message)
        {
        }

        public UnknownErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}