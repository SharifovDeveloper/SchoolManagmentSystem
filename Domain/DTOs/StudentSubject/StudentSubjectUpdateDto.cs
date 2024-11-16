namespace Domain.DTOs.StudentSubject;

public record StudentSubjectUpdateDto(
    int StudentId,
    int SubjectId,
    double Mark
);
