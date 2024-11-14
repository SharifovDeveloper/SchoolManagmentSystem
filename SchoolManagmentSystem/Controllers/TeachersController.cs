using Domain.Common;
using Domain.DTOs.Teacher;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
using Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [HttpGet]
    public async Task<Result<GetBaseResponse<TeacherDto>>> GetTeachersAsync(
        [FromQuery] TeacherResourceParameters teacherResourceParameters)
    {
        var teachers = await _teacherService.GetTeachersAsync(teacherResourceParameters);
        return new Result<GetBaseResponse<TeacherDto>>(teachers);
    }

    [HttpGet("top5highestmarks")]
    public async Task<Result<List<string>>> GetTeachersForTop5StudentsWithHighestMarksAsync()
    {
        var teachers = await _teacherService.GetTeachersForTop5StudentsWithHighestMarksAsync();
        return new Result<List<string>>(teachers);
    }

    [HttpGet("top5lowestmarks")]
    public async Task<Result<List<string>>> GetTeachersForTop5StudentsWithLowestMarksAsync()
    {
        var teachers = await _teacherService.GetTeachersForTop5StudentsWithLowestMarksAsync();
        return new Result<List<string>>(teachers);
    }

    [HttpGet("{teacherId}/subjects")]
    public async Task<Result<List<string>>> GetSubjectsByTeacherIdAsync(int teacherId)
    {
        var subjects = await _teacherService.GetSubjectsByTeacherIdAsync(teacherId);

        return new Result<List<string>>(subjects);
    }

    [HttpGet("{id}", Name = "GetTeacherById")]
    public async Task<Result<TeacherDto>> GetTeacherByIdAsync(int id)
    {
        var teacher = await _teacherService.GetTeacherByIdAsync(id);

        return new Result<TeacherDto>(teacher);
    }

    [HttpPost]
    public async Task<Result<TeacherDto>> PostAsync([FromBody] TeacherCreateDto teacherCreateDto)
    {
        var createdTeacher = await _teacherService.CreateTeacherAsync(teacherCreateDto);

        return new Result<TeacherDto>(createdTeacher);
    }

    [HttpPut("{id}")]
    public async Task<Result<TeacherDto>> PutAsync(int id, [FromBody] TeacherUpdateDto teacherUpdateDto)
    {
        if (id != teacherUpdateDto.Id)
        {
            return new Result<TeacherDto>($"Route id: {id} does not match with parameter id: {teacherUpdateDto.Id}.");
        }

        var updatedTeacher = await _teacherService.UpdateTeacherAsync(teacherUpdateDto);

        return new Result<TeacherDto>(updatedTeacher);
    }

    [HttpDelete("{id}")]
    public async Task<Result<string>> DeleteAsync(int id)
    {
        await _teacherService.DeleteTeacherAsync(id);

        return new Result<string>("Teacher successfully deleted");
    }
}
