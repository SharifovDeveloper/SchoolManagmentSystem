﻿using Domain.Common;
using Domain.DTOs.City;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
using Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet]
    public async Task<Result<GetBaseResponse<CityDto>>> GetCitiesAsync(
        [FromQuery] CityResourceParameters cityResource)
    {
        var cities = await _cityService.GetCitiesAsync(cityResource);

        return new Result<GetBaseResponse<CityDto>>(cities);
    }

    [HttpGet("{id}", Name = "GetCityById")]
    public async Task<Result<CityDto>> GetCityByIdAsync(int id)
    {
        var city = await _cityService.GetCityByIdAsync(id);

        return new Result<CityDto>(city);
    }

    [HttpPost]
    public async Task<Result<CityDto>> PostAsync([FromBody] CityCreateDto cityCreateDto)
    {
        var createdCity = await _cityService.CreateCityAsync(cityCreateDto);

        return new Result<CityDto>(createdCity);
    }

    [HttpPut("{id}")]
    public async Task<Result<CityDto>> PutAsync(int id, [FromBody] CityUpdateDto cityUpdateDto)
    {
        var updatedCity = await _cityService.UpdateCityAsync(id, cityUpdateDto);

        return new Result<CityDto>(updatedCity);
    }

    [HttpDelete("{id}")]
    public async Task<Result<string>> DeleteAsync(int id)
    {
        await _cityService.DeleteCityAsync(id);
        return new Result<string>("City successfully deleted");
    }
}
