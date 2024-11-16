using Domain.DTOs.Subject;
using Domain.DTOs.Teacher;
using Domain.ResourceParameters;
using Domain.Responses;
using System.Xml.Schema;

namespace Domain.Interfaces.Services;

public interface ITeacherService
{
    Task<GetBaseResponse<TeacherDto>> GetTeachersAsync(TeacherResourceParameters teacherResourceParameters);
    Task<List<string>> GetTeachersForTop5StudentsWithHighestMarksAsync();
    Task<List<string>> GetTeachersForTop5StudentsWithLowestMarksAsync();
    Task<List<string>> GetSubjectsByTeacherIdAsync(int teacherId);
    Task<TeacherDto?> GetTeacherByIdAsync(int id);
    Task<TeacherDto> CreateTeacherAsync(TeacherCreateDto teacherCreateDto);
    Task<TeacherDto> UpdateTeacherAsync(int id, TeacherUpdateDto teacherUpdateDto);
    Task DeleteTeacherAsync(int id);
}
