using Domain.DTOs.StudentSubject;
using Domain.ResourceParameters;
using Domain.Responses;

namespace Domain.Interfaces.Services;

public interface IStudentSubjectService
{
    Task<GetBaseResponse<StudentSubjectDto>> GetStudentSubjectsAsync(StudentSubjectResourceParameters studentSubjectResourceParameters);
    Task<StudentSubjectDto?> GetStudentSubjectByIdAsync(int id);
    Task<StudentSubjectDto> CreateStudentSubjectAsync(StudentSubjectCreateDto schoolSubjectCreateDto);
    Task<StudentSubjectDto> UpdateStudentSubjectAsync(int id, StudentSubjectUpdateDto schoolSubjectUpdateDto);
    Task DeleteStudentSubjectAsync(int id);
}
