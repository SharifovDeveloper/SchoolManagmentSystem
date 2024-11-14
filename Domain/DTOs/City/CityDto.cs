namespace Domain.DTOs.City;

public record CityDto(
    int Id,
    string Name,
    DateTime CreatedDate,
    DateTime LastUpdatedDate  
);