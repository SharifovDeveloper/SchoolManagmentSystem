using Domain.Common;
using Domain.DTOs.Student;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
using Domain.Responses;
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
    public async Task<Result<GetBaseResponse<StudentDto>>> GetStudentsAsync(
        [FromQuery] StudentResourceParameters studentResourceParameters)
    {
        var students = await _studentService.GetStudentsAsync(studentResourceParameters);
        return new Result<GetBaseResponse<StudentDto>>(students);
    }
    [HttpGet("top10bysubjectmark")]
    public async Task<Result<List<string>>> GetTop10StudentsBySubjectMarkAsync(int subjectId)
    {
        var students = await _studentService.GetTop10StudentsBySubjectMarkAsync(subjectId);
        return new Result<List<string>>(students);
    }

    [HttpGet("{studentId}/subjects")]
    public async Task<Result<List<string>>> GetSubjectsByStudentIdAsync(int studentId)
    {
        var subjects = await _studentService.GetSubjectsByStudentIdAsync(studentId);
        return new Result<List<string>>(subjects);
    }

    [HttpGet("{id}", Name = "GetStudentById")]
    public async Task<Result<StudentDto>> GetStudentByIdAsync(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);

        return new Result<StudentDto>(student);
    }

    [HttpPost]
    public async Task<Result<StudentDto>> PostAsync([FromBody] StudentCreateDto studentCreateDto)
    {
        var createdStudent = await _studentService.CreateStudentAsync(studentCreateDto);

        return new Result<StudentDto>(createdStudent);
    }

    [HttpPut("{id}")]
    public async Task<Result<StudentDto>> PutAsync(int id, [FromBody] StudentUpdateDto studentUpdateDto)
    {
        if (id != studentUpdateDto.Id)
        {
            return new Result<StudentDto>($"Route id: {id} does not match with parameter id: {studentUpdateDto.Id}.");
        }

        var updatedStudent = await _studentService.UpdateStudentAsync(studentUpdateDto);

        return new Result<StudentDto>(updatedStudent);
    }

    [HttpDelete("{id}")]
    public async Task<Result<string>> DeleteAsync(int id)
    {
        await _studentService.DeleteStudentAsync(id);

        return new Result<string>("Department successfully deleted");
    }
}
