using System;
using System.Runtime.Serialization;

namespace ApiApplication.Core.Exceptions
{
    public class ResourceUnavailableException : Exception
    {
        public ResourceUnavailableException()
        {
        }

        protected ResourceUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ResourceUnavailableException(string message) : base(message)
        {
        }

        public ResourceUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}