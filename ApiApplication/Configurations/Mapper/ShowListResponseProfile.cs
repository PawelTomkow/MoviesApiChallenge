using ApiApplication.Clients.Contracts;
using AutoMapper;
using ProtoDefinitions;

namespace ApiApplication.Configurations.Mapper
{
    public class ShowListResponseProfile : Profile
    {
        public ShowListResponseProfile()
        {
            CreateMap<ShowListResponse, showListResponse>()
                .ReverseMap();
        }
    }
}