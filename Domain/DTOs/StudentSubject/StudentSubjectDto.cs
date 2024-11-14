namespace Domain.DTOs.StudentSubject;

public record StudentSubjectDto(
     int Id,
     int StudentId,
     int SubjectId,
     double Mark,
     DateTime CreatedDate,
     DateTime LastUpdatedDate
);
