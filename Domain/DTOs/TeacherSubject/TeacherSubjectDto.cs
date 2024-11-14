namespace Domain.DTOs.TeacherSubject;

public record TeacherSubjectDto(
     int Id,
     int TeacherId,
     int SubjectId,
     DateTime CreatedDate,
     DateTime LastUpdatedDate
);
