using Domain.DTOs.TeacherSubject;
using Domain.ResourceParameters;
using Domain.Responses;

namespace Domain.Interfaces.Services;

public interface ITeacherSubjectService
{
    Task<GetBaseResponse<TeacherSubjectDto>> GetTeacherSubjectsAsync(TeacherSubjectResourceParameters teacherSubjectResourceParameters);
    Task<TeacherSubjectDto?> GetTeacherSubjectByIdAsync(int id);
    Task<TeacherSubjectDto> CreateTeacherSubjectAsync(TeacherSubjectCreateDto teacherSubjectCreateDto);
    Task<TeacherSubjectDto> UpdateTeacherSubjectAsync(int id, TeacherSubjectUpdateDto teacherSubjectUpdateDto);
    Task DeleteTeacherSubjectAsync(int id);
}
