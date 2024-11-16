using Domain.DTOs.City;
using Domain.ResourceParameters;
using Domain.Responses;

namespace Domain.Interfaces.Services;

public interface ICityService
{
    Task<GetBaseResponse<CityDto>> GetCitiesAsync(CityResourceParameters cityResourceParameters);
    Task<CityDto?> GetCityByIdAsync(int id);
    Task<CityDto> CreateCityAsync(CityCreateDto cityCreateDto);
    Task<CityDto> UpdateCityAsync(int id, CityUpdateDto cityUpdateDto);
    Task DeleteCityAsync(int id);
}
