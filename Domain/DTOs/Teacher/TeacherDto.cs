using Domain.Enums;

namespace Domain.DTOs.Teacher;

public record TeacherDto(
    int Id,
    string Name,
    int CityId,
    DateTime BirthDate,
    Gender Gender,
    DateTime CreatedDate,
    DateTime LastUpdatedDate
);
