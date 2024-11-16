using Domain.Common;
using Domain.DTOs.Subject;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
using Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Domain.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    public async Task<Result<GetBaseResponse<SubjectDto>>> GetSubjectsAsync(
        [FromQuery] SubjectResourceParameters subjectResource)
    {
        var subjects = await _subjectService.GetSubjectsAsync(subjectResource);

        return new Result<GetBaseResponse<SubjectDto>>(subjects);
    }

    [HttpGet("{id}", Name = "GetSubjectById")]
    public async Task<Result<SubjectDto>> GetSubjectByIdAsync(int id)
    {
        var subject = await _subjectService.GetSubjectByIdAsync(id);

        return new Result<SubjectDto>(subject);
    }

    [HttpPost]
    public async Task<Result<SubjectDto>> PostAsync([FromBody] SubjectCreateDto subjectCreateDto)
    {
        var createdSubject = await _subjectService.CreateSubjectAsync(subjectCreateDto);

        return new Result<SubjectDto>(createdSubject);
    }

    [HttpPut("{id}")]
    public async Task<Result<SubjectDto>> PutAsync(int id, [FromBody] SubjectUpdateDto subjectUpdateDto)
    {
        var updatedSubject = await _subjectService.UpdateSubjectAsync(id, subjectUpdateDto);

        return new Result<SubjectDto>(updatedSubject);
    }

    [HttpDelete("{id}")]
    public async Task<Result<string>> DeleteAsync(int id)
    {
        await _subjectService.DeleteSubjectAsync(id);

        return new Result<string>("Subject successfully deleted");
    }
}
