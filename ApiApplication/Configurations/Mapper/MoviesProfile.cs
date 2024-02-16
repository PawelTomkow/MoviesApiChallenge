using ApiApplication.Clients.Contracts;
using ApiApplication.Core.Models;
using AutoMapper;

namespace ApiApplication.Configurations.Mapper
{
    public class MoviesProfile : Profile
    {
        public MoviesProfile()
        {
            CreateMap<ShowResponse, Movie>()
                .ReverseMap();
        }
    }
}