namespace Domain.DTOs.TeacherSubject;

public record TeacherSubjectUpdateDto(
    int Id,
    int TeacherId,
    int SubjectId
);
