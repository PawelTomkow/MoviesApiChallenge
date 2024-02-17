using System;

namespace ApiApplication.Core.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime DateTimeNow => DateTime.Now;
    }
}