using Domain.Enums;

namespace Domain.DTOs.Teacher;

public record TeacherUpdateDto(
    string Name,
    int CityId,
    DateTime BirthDate,
    Gender Gender
);
