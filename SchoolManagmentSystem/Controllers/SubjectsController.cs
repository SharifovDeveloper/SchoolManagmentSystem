using Domain.DTOs.Subject;
using Domain.Interfaces.Services;
using Domain.ResourceParameters;
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
    public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjectsAsync(
        [FromQuery] SubjectResourceParameters subjectResource)
    {
        var subjects = await _subjectService.GetSubjectsAsync(subjectResource);

        return Ok(subjects);
    }

    [HttpGet("{id}", Name = "GetSubjectById")]
    public async Task<ActionResult<SubjectDto>> GetSubjectByIdAsync(int id)
    {
        var subject = await _subjectService.GetSubjectByIdAsync(id);

        return Ok(subject);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] SubjectCreateDto subjectCreateDto)
    {
        var createdSubject = await _subjectService.CreateSubjectAsync(subjectCreateDto);

        return CreatedAtAction("GetSubjectById", new { id = createdSubject.Id }, createdSubject);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> PutAsync(int id, [FromBody] SubjectUpdateDto subjectUpdateDto)
    {
        if (id != subjectUpdateDto.Id)
        {
            return BadRequest(
                $"Route id: {id} does not match with parameter id: {subjectUpdateDto.Id}.");
        }

        var updatedSubject = await _subjectService.UpdateSubjectAsync(subjectUpdateDto);

        return Ok(updatedSubject);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        await _subjectService.DeleteSubjectAsync(id);

        return NoContent();
    }
}
