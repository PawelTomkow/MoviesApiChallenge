using System.Collections.Generic;
using ApiApplication.Core.Models;

namespace ApiApplication.Controllers.Contracts.Auditoriums
{
    public class GetAllAuditoriumsResponse
    {
        public List<Auditorium> Auditoriums { get; set; }
    }
}