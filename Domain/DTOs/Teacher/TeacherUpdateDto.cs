using Domain.Enums;

namespace Domain.DTOs.Teacher;

public record TeacherUpdateDto(
    int Id,
    string Name,
    int CityId,
    DateTime BirthDate,
    Gender Gender
);
