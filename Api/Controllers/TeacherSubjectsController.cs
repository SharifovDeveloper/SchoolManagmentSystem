using Domain.Common;
using Domain.DTOs.TeacherSubject;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
using Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeacherSubjectsController : ControllerBase
{
    private readonly ITeacherSubjectService _teacherSubjectService;

    public TeacherSubjectsController(ITeacherSubjectService teacherSubjectService)
    {
        _teacherSubjectService = teacherSubjectService;
    }

    [HttpGet]
    public async Task<Result<GetBaseResponse<TeacherSubjectDto>>> GetTeacherSubjectsAsync(
        [FromQuery] TeacherSubjectResourceParameters teacherSubjectResourceParameters)
    {
        var teacherSubjects = await _teacherSubjectService.GetTeacherSubjectsAsync(teacherSubjectResourceParameters);
       
        return new Result<GetBaseResponse<TeacherSubjectDto>>(teacherSubjects);
    }

    [HttpGet("{id}", Name = "GetTeacherSubjectById")]
    public async Task<Result<TeacherSubjectDto>> GetTeacherSubjectByIdAsync(int id)
    {
        var teacherSubject = await _teacherSubjectService.GetTeacherSubjectByIdAsync(id);

        return new Result<TeacherSubjectDto>(teacherSubject);
    }

    [HttpPost]
    public async Task<Result<TeacherSubjectDto>> PostAsync([FromBody] TeacherSubjectCreateDto teacherSubjectCreateDto)
    {
        var createdTeacherSubject = await _teacherSubjectService.CreateTeacherSubjectAsync(teacherSubjectCreateDto);

        return new Result<TeacherSubjectDto>(createdTeacherSubject);
    }

    [HttpPut("{id}")]
    public async Task<Result<TeacherSubjectDto>> PutAsync(int id, [FromBody] TeacherSubjectUpdateDto teacherSubjectUpdateDto)
    {
        var updatedTeacherSubject = await _teacherSubjectService.UpdateTeacherSubjectAsync(id, teacherSubjectUpdateDto);

        return new Result<TeacherSubjectDto>(updatedTeacherSubject);
    }

    [HttpDelete("{id}")]
    public async Task<Result<string>> DeleteAsync(int id)
    {
        await _teacherSubjectService.DeleteTeacherSubjectAsync(id);

        return new Result<string>("Teacher successfully deleted");
    }
}
