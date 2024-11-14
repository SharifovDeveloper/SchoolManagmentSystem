using Domain.Enums;

namespace Domain.DTOs.Teacher;

public record TeacherCreateDto(
    string Name,
    int CityId,
    DateTime BirthDate,
    Gender Gender
);
