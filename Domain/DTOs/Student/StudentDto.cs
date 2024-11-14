using Domain.Enums;

namespace Domain.DTOs.Student;

public record StudentDto(
   int Id,
   string Name,
   int CityId,
   DateTime BirthDate,
   Gender Gender,
   int CurrentGradeLevel,
   int DepartmentId,
   DateTime CreatedDate,
   DateTime LastUpdatedDate
);
