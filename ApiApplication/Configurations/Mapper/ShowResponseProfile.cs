using ApiApplication.Clients.Contracts;
using AutoMapper;
using ProtoDefinitions;

namespace ApiApplication.Configurations.Mapper
{
    public class ShowResponseProfile : Profile
    {
        public ShowResponseProfile()
        {
            CreateMap<ShowResponse, showResponse>()
                .ReverseMap();
        }
    }
}