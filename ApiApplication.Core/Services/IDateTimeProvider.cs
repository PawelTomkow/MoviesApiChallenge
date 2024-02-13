using System;

namespace ApiApplication.Core.Services
{
    public interface IDateTimeProvider
    {
        public DateTime DateTimeNow { get; }
    }
}