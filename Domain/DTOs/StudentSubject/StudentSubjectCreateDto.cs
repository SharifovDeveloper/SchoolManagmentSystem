namespace Domain.DTOs.StudentSubject;

public record StudentSubjectCreateDto(
     int StudentId,
     int SubjectId,
     double Mark
);
