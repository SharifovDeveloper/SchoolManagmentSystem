using Domain.DTOs.Subject;
using Domain.ResourceParameters;
using Domain.Responses;

namespace Domain.Interfaces.Services;

public interface ISubjectService
{
    Task<GetBaseResponse<SubjectDto>> GetSubjectsAsync(SubjectResourceParameters sbjectResourceParameters);
    Task<SubjectDto?> GetSubjectByIdAsync(int id);
    Task<SubjectDto> CreateSubjectAsync(SubjectCreateDto subjectCreateDto);
    Task<SubjectDto> UpdateSubjectAsync(SubjectUpdateDto subjectUpdateDto);
    Task DeleteSubjectAsync(int id);
}
