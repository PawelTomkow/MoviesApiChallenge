using System;
using System.Runtime.Serialization;

namespace ApiApplication.Core.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public string PropertyValue { get; }
        public string PropertyName { get; }
        public Type DomainType { get; }

        public ResourceNotFoundException()
        {
        }

        protected ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ResourceNotFoundException(string message) : base(message)
        {
        }
        
        public ResourceNotFoundException(Type domainType, string propertyName, string propertyValue, string message = "ResourceNotFoundException thrown.") : base(message)
        {
            PropertyValue = propertyValue;
            PropertyName = propertyName;
            DomainType = domainType;
        }

        public ResourceNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}