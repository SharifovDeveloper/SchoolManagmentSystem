namespace Domain.DTOs.Department;

public record DepartmentDto(
    int Id,
    string Name,
    DateTime CreatedDate,
    DateTime LastUpdatedDate
);

