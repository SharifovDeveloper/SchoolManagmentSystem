using Domain.DTOs.Student;
using Domain.ResourceParameters;
using Domain.Responses;

namespace Domain.Interfaces.Services;

public interface IStudentService
{
    Task<GetBaseResponse<StudentDto>> GetStudentsAsync(StudentResourceParameters studentResourceParameters);
    Task<List<string>> GetTop10StudentsBySubjectMarkAsync(int subjectId);
    Task<List<string>> GetSubjectsByStudentIdAsync(int studentId);
    Task<StudentDto?> GetStudentByIdAsync(int id);
    Task<StudentDto> CreateStudentAsync(StudentCreateDto studentCreateDto);
    Task<StudentDto> UpdateStudentAsync(int id, StudentUpdateDto studentUpdateDto);
    Task DeleteStudentAsync(int id);
}
