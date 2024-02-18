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
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Rank))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.FullTitle, opt => opt.MapFrom(src => src.FullTitle))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.Crew, opt => opt.MapFrom(src => src.Crew))
                .ForMember(dest => dest.ImDbRating, opt => opt.MapFrom(src => src.ImDbRating))
                .ForMember(dest => dest.ImDbRatingCount, opt => opt.MapFrom(src => src.ImDbRatingCount));
            
            CreateMap<Movie, ShowResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ImdbId));

        }
    }
}