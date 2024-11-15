using AutoMapper;
using Domain.DTOs.City;
using Domain.Entities;
using Domain.Helpers;
using Domain.Interfaces.Services;
using Domain.Pagniation;
using Domain.ResourceParameters;
using Domain.Responses;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class CityService : ICityService
{
    private readonly IMapper _mapper;
    private readonly SchoolManagmentDbContext _context;

    public CityService(IMapper mapper, SchoolManagmentDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetBaseResponse<CityDto>> GetCitiesAsync(CityResourceParameters cityResourceParameters)
    {
        var query = _context.Cities
            .AsNoTracking()
            .AsQueryable();

        query = query.ApplyDateFilters(
        cityResourceParameters.CreatedDateFrom,
        cityResourceParameters.CreatedDateTo,
        cityResourceParameters.LastUpdatedDateFrom,
        cityResourceParameters.LastUpdatedDateTo);

        query = query.ApplyIsDeletedFilter(cityResourceParameters.IsDeleted);

        if (!string.IsNullOrWhiteSpace(cityResourceParameters.SearchString))
        {
            query = query.Where(x => x.Name.Contains(cityResourceParameters.SearchString));
        }

        if (!string.IsNullOrEmpty(cityResourceParameters.OrderBy))
        {
            query = cityResourceParameters.OrderBy.ToLowerInvariant() switch
            {
                "name" => query.OrderBy(x => x.Name),
                "namedesc" => query.OrderByDescending(x => x.Name),
                _ => query.OrderBy(x => x.Id),
            };
        }

        var cities = await query.ToPaginatedListAsync(cityResourceParameters.PageSize, cityResourceParameters.PageNumber);

        var cityDtos = _mapper.Map<List<CityDto>>(cities);

        var paginatedResult = new PaginatedList<CityDto>(cityDtos, cities.TotalCount, cities.CurrentPage, cities.PageSize);

        return paginatedResult.ToResponse();
    }

    public async Task<CityDto?> GetCityByIdAsync(int id)
    {
        var city = await _context.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        var cityDto = _mapper.Map<CityDto>(city);

        return cityDto;
    }

    public async Task<CityDto> CreateCityAsync(CityCreateDto cityCreateDto)
    {
        var cityEntity = _mapper.Map<City>(cityCreateDto);

        cityEntity.CreatedDate = DateTime.Now;
        cityEntity.LastUpdatedDate = DateTime.Now;

        await _context.Cities.AddAsync(cityEntity);
        await _context.SaveChangesAsync();

        var cityDto = _mapper.Map<CityDto>(cityEntity);

        return cityDto;
    }

    public async Task<CityDto> UpdateCityAsync(CityUpdateDto cityUpdateDto)
    {
        var cityEntity = await _context.Cities.FindAsync(cityUpdateDto.Id);

        _mapper.Map(cityUpdateDto, cityEntity);

        cityEntity.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        var cityDto = _mapper.Map<CityDto>(cityEntity);
        return cityDto;
    }

    public async Task DeleteCityAsync(int id)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(x => x.Id == id);

        city.IsDeleted = true;

        _context.Cities.Update(city);
        await _context.SaveChangesAsync();
    }
}
