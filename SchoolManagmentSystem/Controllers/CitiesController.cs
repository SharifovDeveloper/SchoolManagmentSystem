using Domain.DTOs.City;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
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
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCitiesAsync(
        [FromQuery] CityResourceParameters cityResource)
    {
        var cityReviews = await _cityService.GetCitiesAsync(cityResource); ;

        return Ok(cityReviews);
    }

    [HttpGet("{id}", Name = "GetCityById")]
    public async Task<ActionResult<CityDto>> GetCityByIdAsync(int id)
    {
        var city = await _cityService.GetCityByIdAsync(id);

        return Ok(city);
    }
    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] CityCreateDto forCreateDto)
    {
        var createdCity = await _cityService.CreateCityAsync(forCreateDto);

        return CreatedAtAction("GetCityById", new { id = createdCity.Id }, createdCity);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> PutAsync(int id, [FromBody] CityUpdateDto forUpdateDto)
    {
        if (id != forUpdateDto.Id)
        {
            return BadRequest(
                $"Route id: {id} does not match with parameter id: {forUpdateDto.Id}.");
        }

        var updateCity = await _cityService.UpdateCityAsync(forUpdateDto);

        return Ok(updateCity);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        await _cityService.DeleteCityAsync(id);

        return NoContent();
    }
}
