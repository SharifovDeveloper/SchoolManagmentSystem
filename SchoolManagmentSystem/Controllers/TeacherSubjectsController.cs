using Domain.DTOs.TeacherSubject;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
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
    public async Task<ActionResult<IEnumerable<TeacherSubjectDto>>> GetTeacherSubjectsAsync(
        [FromQuery] TeacherSubjectResourceParameters teacherSubjectResourceParameters)
    {
        var teacherSubjects = await _teacherSubjectService.GetTeacherSubjectsAsync(teacherSubjectResourceParameters);
        return Ok(teacherSubjects);
    }

    [HttpGet("{id}", Name = "GetTeacherSubjectById")]
    public async Task<ActionResult<TeacherSubjectDto>> GetTeacherSubjectByIdAsync(int id)
    {
        var teacherSubject = await _teacherSubjectService.GetTeacherSubjectByIdAsync(id);

        if (teacherSubject == null)
        {
            return NotFound(new { Message = $"Teacher subject with ID {id} not found." });
        }

        return Ok(teacherSubject);
    }

    [HttpPost]
    public async Task<ActionResult<TeacherSubjectDto>> PostAsync([FromBody] TeacherSubjectCreateDto teacherSubjectCreateDto)
    {
        var createdTeacherSubject = await _teacherSubjectService.CreateTeacherSubjectAsync(teacherSubjectCreateDto);

        return CreatedAtAction("GetTeacherSubjectById", new { id = createdTeacherSubject.Id }, createdTeacherSubject);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TeacherSubjectDto>> PutAsync(int id, [FromBody] TeacherSubjectUpdateDto teacherSubjectUpdateDto)
    {
        if (id != teacherSubjectUpdateDto.Id)
        {
            return BadRequest($"Route id: {id} does not match with parameter id: {teacherSubjectUpdateDto.Id}.");
        }

        var updatedTeacherSubject = await _teacherSubjectService.UpdateTeacherSubjectAsync(teacherSubjectUpdateDto);

        return Ok(updatedTeacherSubject);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        await _teacherSubjectService.DeleteTeacherSubjectAsync(id);

        return NoContent();
    }
}
