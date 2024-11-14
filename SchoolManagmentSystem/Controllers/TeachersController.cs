using Domain.DTOs.Subject;
using Domain.DTOs.Teacher;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
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
    public async Task<ActionResult<IEnumerable<TeacherDto>>> GetTeachersAsync(
        [FromQuery] TeacherResourceParameters teacherResourceParameters)
    {
        var teachers = await _teacherService.GetTeachersAsync(teacherResourceParameters);
        return Ok(teachers);
    }

    [HttpGet("top5highestmarks")]
    public async Task<ActionResult<IEnumerable<TeacherDto>>> GetTeachersForTop5StudentsWithHighestMarksAsync()
    {
        var teachers = await _teacherService.GetTeachersForTop5StudentsWithHighestMarksAsync();
        return Ok(teachers);
    }

    [HttpGet("top5lowestmarks")]
    public async Task<ActionResult<IEnumerable<TeacherDto>>> GetTeachersForTop5StudentsWithLowestMarksAsync()
    {
        var teachers = await _teacherService.GetTeachersForTop5StudentsWithLowestMarksAsync();
        return Ok(teachers);
    }

    [HttpGet("{teacherId}/subjects")]
    public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjectsByTeacherIdAsync(int teacherId)
    {
        var subjects = await _teacherService.GetSubjectsByTeacherIdAsync(teacherId);

        return Ok(subjects);
    }

    [HttpGet("{id}", Name = "GetTeacherById")]
    public async Task<ActionResult<TeacherDto>> GetTeacherByIdAsync(int id)
    {
        var teacher = await _teacherService.GetTeacherByIdAsync(id);

        return Ok(teacher);
    }

    [HttpPost]
    public async Task<ActionResult<TeacherDto>> PostAsync([FromBody] TeacherCreateDto teacherCreateDto)
    {
        var createdTeacher = await _teacherService.CreateTeacherAsync(teacherCreateDto);

        return CreatedAtAction("GetTeacherById", new { id = createdTeacher.Id }, createdTeacher);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TeacherDto>> PutAsync(int id, [FromBody] TeacherUpdateDto teacherUpdateDto)
    {
        if (id != teacherUpdateDto.Id)
        {
            return BadRequest($"Route id: {id} does not match with parameter id: {teacherUpdateDto.Id}.");
        }

        var updatedTeacher = await _teacherService.UpdateTeacherAsync(teacherUpdateDto);

        return Ok(updatedTeacher);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        await _teacherService.DeleteTeacherAsync(id);

        return NoContent();
    }
}
