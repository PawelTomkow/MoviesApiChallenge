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
                .ForMember(dest => dest.ImdbId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            
            CreateMap<Movie, ShowResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ImdbId));

        }
    }
}