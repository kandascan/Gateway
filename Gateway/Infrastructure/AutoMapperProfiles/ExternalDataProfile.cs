using AutoMapper;
using Gateway.ApplicationCore.DTOs;
using Gateway.ApplicationCore.Models;

namespace Gateway.Infrastructure.AutoMapperProfiles
{
    public class ExternalDataProfile : Profile
    {
        public ExternalDataProfile()
        {
            CreateMap<ProductDto, Produkt>()
                .ForMember(dest => dest.Identyfikator, opt => opt.MapFrom(src => src.Id != null ? int.Parse(src.Id) : 0))
                .ForMember(dest => dest.Nazwa, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Kolor, opt => opt.MapFrom(src => src.Data != null ? src.Data.Color : "Brak koloru"));
        }
    }
}
