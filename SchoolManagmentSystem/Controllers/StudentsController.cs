using Domain.DTOs.Student;
using Domain.DTOs.Subject;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudentsAsync(
        [FromQuery] StudentResourceParameters studentResourceParameters)
    {
        var students = await _studentService.GetStudentsAsync(studentResourceParameters);
        return Ok(students);
    }
    [HttpGet("top10bysubjectmark")]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetTop10StudentsBySubjectMarkAsync(int subjectId)
    {
        var teachers = await _studentService.GetTop10StudentsBySubjectMarkAsync(subjectId);
        return Ok(teachers);
    }

    [HttpGet("{studentId}/subjects")]
    public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjectsByStudentIdAsync(int studentId)
    {
        var subjects = await _studentService.GetSubjectsByStudentIdAsync(studentId);

        if (subjects == null || !subjects.Any())
        {
            return NotFound(new { Message = $"No subjects found for student with ID {studentId}." });
        }

        return Ok(subjects);
    }

    [HttpGet("{id}", Name = "GetStudentById")]
    public async Task<ActionResult<StudentDto>> GetStudentByIdAsync(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);

        return Ok(student);
    }

    [HttpPost]
    public async Task<ActionResult<StudentDto>> PostAsync([FromBody] StudentCreateDto studentCreateDto)
    {
        var createdStudent = await _studentService.CreateStudentAsync(studentCreateDto);

        return CreatedAtAction("GetStudentById", new { id = createdStudent.Id }, createdStudent);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StudentDto>> PutAsync(int id, [FromBody] StudentUpdateDto studentUpdateDto)
    {
        if (id != studentUpdateDto.Id)
        {
            return BadRequest($"Route id: {id} does not match with parameter id: {studentUpdateDto.Id}.");
        }

        var updatedStudent = await _studentService.UpdateStudentAsync(studentUpdateDto);

        return Ok(updatedStudent);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        await _studentService.DeleteStudentAsync(id);

        return NoContent();
    }
}
