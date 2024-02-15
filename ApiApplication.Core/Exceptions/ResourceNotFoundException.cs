using System;
using System.Runtime.Serialization;

namespace ApiApplication.Core.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException()
        {
        }

        protected ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ResourceNotFoundException(string message) : base(message)
        {
        }

        public ResourceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}