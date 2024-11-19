using AutoMapper;
using Domain.DTOs.City;
using Domain.Entities;
using Domain.Exeptions;
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

        query = ApplyFilters(query, cityResourceParameters);

        if (!string.IsNullOrWhiteSpace(cityResourceParameters.SearchString))
        {
            query = query.Where(c => c.Name.Contains(cityResourceParameters.SearchString));
        }

        if (!string.IsNullOrEmpty(cityResourceParameters.OrderBy))
        {
            query = ApplyOrdering(query, cityResourceParameters);
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
            .FirstOrDefaultAsync(c => c.Id == id);

        if (city == null)
            throw new EntityNotFoundException($"City with ID {id} was not found.");

        return _mapper.Map<CityDto>(city);
    }

    public async Task<CityDto> CreateCityAsync(CityCreateDto cityCreateDto)
    {
        var city = _mapper.Map<City>(cityCreateDto);
        city.CreatedDate = DateTime.Now;
        city.LastUpdatedDate = DateTime.Now;

        await _context.Cities.AddAsync(city);
        await _context.SaveChangesAsync();

        return _mapper.Map<CityDto>(city);
    }

    public async Task<CityDto> UpdateCityAsync(int id, CityUpdateDto cityUpdateDto)
    {
        var city = await _context.Cities.FindAsync(id);

        if (city == null)
            throw new EntityNotFoundException($"City with ID {id} was not found.");

        _mapper.Map(cityUpdateDto, city);
        city.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        return _mapper.Map<CityDto>(city);
    }

    public async Task DeleteCityAsync(int id)
    {
        var city = await _context.Cities.FindAsync(id);

        if (city == null)
            throw new EntityNotFoundException($"City with ID {id} was not found.");

        city.IsDeleted = true;

        _context.Cities.Update(city);
        await _context.SaveChangesAsync();
    }

    private IQueryable<City> ApplyFilters(IQueryable<City> query, CityResourceParameters parameters)
    {
        query = query.ApplyDateFilters(
            parameters.CreatedDateFrom, parameters.CreatedDateTo,
            parameters.LastUpdatedDateFrom, parameters.LastUpdatedDateTo
        );
        query = query.ApplyIsDeletedFilter(parameters.IsDeleted);

        return query;
    }

    private IQueryable<City> ApplyOrdering(IQueryable<City> query, CityResourceParameters parameters)
    {
        return parameters.OrderBy.ToLowerInvariant() switch
        {
            "name" => query.OrderBy(c => c.Name),
            "namedesc" => query.OrderByDescending(c => c.Name),
            _ => query.OrderBy(c => c.Id),
        };
    }
}
