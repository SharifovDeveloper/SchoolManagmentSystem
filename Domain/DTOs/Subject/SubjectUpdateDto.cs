namespace Domain.DTOs.Subject;

public record SubjectUpdateDto(
    int Id,
    string Name,
    int GradeLevel
);
