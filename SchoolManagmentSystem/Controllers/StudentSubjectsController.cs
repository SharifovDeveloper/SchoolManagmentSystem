using Domain.DTOs.StudentSubject;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
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
    public async Task<ActionResult<IEnumerable<StudentSubjectDto>>> GetStudentSubjectsAsync(
        [FromQuery] StudentSubjectResourceParameters studentSubjectResourceParameters)
    {
        var studentSubjects = await _studentSubjectService.GetStudentSubjectsAsync(studentSubjectResourceParameters);
        return Ok(studentSubjects);
    }

    [HttpGet("{id}", Name = "GetStudentSubjectById")]
    public async Task<ActionResult<StudentSubjectDto>> GetStudentSubjectByIdAsync(int id)
    {
        var studentSubject = await _studentSubjectService.GetStudentSubjectByIdAsync(id);

        return Ok(studentSubject);
    }

    [HttpPost]
    public async Task<ActionResult<StudentSubjectDto>> PostAsync([FromBody] StudentSubjectCreateDto studentSubjectCreateDto)
    {
        var createdStudentSubject = await _studentSubjectService.CreateStudentSubjectAsync(studentSubjectCreateDto);

        return CreatedAtAction("GetStudentSubjectById", new { id = createdStudentSubject.Id }, createdStudentSubject);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StudentSubjectDto>> PutAsync(int id, [FromBody] StudentSubjectUpdateDto studentSubjectUpdateDto)
    {
        if (id != studentSubjectUpdateDto.Id)
        {
            return BadRequest($"Route id: {id} does not match with parameter id: {studentSubjectUpdateDto.Id}.");
        }

        var updatedStudentSubject = await _studentSubjectService.UpdateStudentSubjectAsync(studentSubjectUpdateDto);

        return Ok(updatedStudentSubject);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        await _studentSubjectService.DeleteStudentSubjectAsync(id);

        return NoContent();
    }
}
