using AutoMapper;
using Domain.DTOs.City;
using Domain.Entities;

namespace Domain.Mappings;

public class CityMappings : Profile
{
    public CityMappings()
    {
        CreateMap<City, CityDto>();
        CreateMap<CityCreateDto, City>();
        CreateMap<CityUpdateDto, City>();
    }
}
