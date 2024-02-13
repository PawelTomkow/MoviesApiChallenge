using ApiApplication.Core.Models;
using ApiApplication.Database.Entities;
using AutoMapper;

namespace ApiApplication.Configurations.Mapper
{
    public class EntitiesToDomainProfile : Profile
    {
        public EntitiesToDomainProfile()
        {
            CreateMap<Auditorium, AuditoriumEntity>()
                .ReverseMap();

            CreateMap<Movie, MovieEntity>()
                .ReverseMap();

            CreateMap<Seat, SeatEntity>()
                .ReverseMap();

            CreateMap<Showtime, ShowtimeEntity>()
                .ReverseMap();

            CreateMap<Ticket, TicketEntity>()
                .ReverseMap();
        }
    }
}