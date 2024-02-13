using System;
using ApiApplication.Core.Services;

namespace ApiApplication.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime DateTimeNow => DateTime.Now;
    }
}