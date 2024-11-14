namespace Domain.DTOs.Subject;

public record SubjectDto(
    int Id,
    string Name,
    int GradeLevel,
    DateTime CreatedDate,
    DateTime LastUpdatedDate
);
