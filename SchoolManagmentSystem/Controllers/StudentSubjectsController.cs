using Domain.Common;
using Domain.DTOs.StudentSubject;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
using Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentSubjectsController : ControllerBase
{
    private readonly IStudentSubjectService _studentSubjectService;

    public StudentSubjectsController(IStudentSubjectService studentSubjectService)
    {
        _studentSubjectService = studentSubjectService;
    }

    [HttpGet]
    public async Task<Result<GetBaseResponse<StudentSubjectDto>>> GetStudentSubjectsAsync(
        [FromQuery] StudentSubjectResourceParameters studentSubjectResourceParameters)
    {
        var studentSubjects = await _studentSubjectService.GetStudentSubjectsAsync(studentSubjectResourceParameters);
       
        return new Result<GetBaseResponse<StudentSubjectDto>>(studentSubjects);
    }

    [HttpGet("{id}", Name = "GetStudentSubjectById")]
    public async Task<Result<StudentSubjectDto>> GetStudentSubjectByIdAsync(int id)
    {
        var studentSubject = await _studentSubjectService.GetStudentSubjectByIdAsync(id);

        return new Result<StudentSubjectDto>(studentSubject);
    }

    [HttpPost]
    public async Task<Result<StudentSubjectDto>> PostAsync([FromBody] StudentSubjectCreateDto studentSubjectCreateDto)
    {
        var createdStudentSubject = await _studentSubjectService.CreateStudentSubjectAsync(studentSubjectCreateDto);

        return new Result<StudentSubjectDto>(createdStudentSubject);
    }

    [HttpPut("{id}")]
    public async Task<Result<StudentSubjectDto>> PutAsync(int id, [FromBody] StudentSubjectUpdateDto studentSubjectUpdateDto)
    {
        var updatedStudentSubject = await _studentSubjectService.UpdateStudentSubjectAsync(id, studentSubjectUpdateDto);

        return new Result<StudentSubjectDto>(updatedStudentSubject);
    }

    [HttpDelete("{id}")]
    public async Task<Result<string>> DeleteAsync(int id)
    {
        await _studentSubjectService.DeleteStudentSubjectAsync(id);

        return new Result<string>("Student subject successfully deleted");
    }
}
