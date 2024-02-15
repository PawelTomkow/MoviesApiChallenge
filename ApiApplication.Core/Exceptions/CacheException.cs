using System;
using System.Runtime.Serialization;

namespace ApiApplication.Core.Exceptions
{
    public class CacheException : Exception
    {
        public CacheException()
        {
        }

        protected CacheException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CacheException(string message) : base(message)
        {
        }

        public CacheException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}