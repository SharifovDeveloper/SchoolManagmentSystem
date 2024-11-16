using AutoMapper;
using Domain.DTOs.StudentSubject;
using Domain.Entities;
using Domain.Helpers;
using Domain.Interfaces.Services;
using Domain.Pagniation;
using Domain.ResourceParameters;
using Domain.Responses;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class StudentSubjectService : IStudentSubjectService
{
    private readonly IMapper _mapper;
    private readonly SchoolManagmentDbContext _context;

    public StudentSubjectService(IMapper mapper, SchoolManagmentDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetBaseResponse<StudentSubjectDto>> GetStudentSubjectsAsync(StudentSubjectResourceParameters studentSubjectResourceParameters)
    {
        if (studentSubjectResourceParameters.StudentId.HasValue || studentSubjectResourceParameters.SubjectId.HasValue)
        {
            await ValidateStudentAndSubjectAsync(studentSubjectResourceParameters.StudentId, studentSubjectResourceParameters.SubjectId);
        }

        var query = _context.StudentSubjects
            .AsNoTracking()
            .Include(ss => ss.Student)
            .Include(ss => ss.Subject)
            .AsQueryable();

        query = query.ApplyDateFilters(
        studentSubjectResourceParameters.CreatedDateFrom,
        studentSubjectResourceParameters.CreatedDateTo,
        studentSubjectResourceParameters.LastUpdatedDateFrom,
        studentSubjectResourceParameters.LastUpdatedDateTo);

        query = query.ApplyIsDeletedFilter(studentSubjectResourceParameters.IsDeleted);

        if (studentSubjectResourceParameters.StudentId.HasValue)
        {
            query = query.Where(ss => ss.StudentId == studentSubjectResourceParameters.StudentId.Value);
        }

        if (studentSubjectResourceParameters.SubjectId.HasValue)
        {
            query = query.Where(ss => ss.SubjectId == studentSubjectResourceParameters.SubjectId.Value);
        }

        if (studentSubjectResourceParameters.MinMark.HasValue)
        {
            query = query.Where(ss => ss.Mark >= studentSubjectResourceParameters.MinMark.Value);
        }

        if (studentSubjectResourceParameters.MaxMark.HasValue)
        {
            query = query.Where(ss => ss.Mark <= studentSubjectResourceParameters.MaxMark.Value);
        }

        if (!string.IsNullOrEmpty(studentSubjectResourceParameters.OrderBy))
        {
            query = studentSubjectResourceParameters.OrderBy.ToLowerInvariant() switch
            {
                "mark" => query.OrderBy(ss => ss.Mark),
                "markdesc" => query.OrderByDescending(ss => ss.Mark),
                _ => query.OrderBy(ss => ss.Student.Id),
            };
        }

        var studentSubjects = await query.ToPaginatedListAsync(studentSubjectResourceParameters.PageSize, studentSubjectResourceParameters.PageNumber);

        var studentSubjectDtos = _mapper.Map<List<StudentSubjectDto>>(studentSubjects);

        var paginatedResult = new PaginatedList<StudentSubjectDto>(studentSubjectDtos, studentSubjects.TotalCount, studentSubjects.CurrentPage, studentSubjects.PageSize);

        return paginatedResult.ToResponse();
    }

    public async Task<StudentSubjectDto> GetStudentSubjectByIdAsync(int id)
    {
        var studentSubject = await _context.StudentSubjects
            .AsNoTracking()
            .Include(ss => ss.Student)
            .Include(ss => ss.Subject)
            .FirstOrDefaultAsync(ss => ss.Id == id);

        if (studentSubject is null)
            throw new KeyNotFoundException($"StudentSubject with ID {id} was not found.");

        var studentSubjectDto = _mapper.Map<StudentSubjectDto>(studentSubject);

        return studentSubjectDto;
    }

    public async Task<StudentSubjectDto> CreateStudentSubjectAsync(StudentSubjectCreateDto studentSubjectCreateDto)
    {
        await ValidateStudentAndSubjectAsync(studentSubjectCreateDto.StudentId, studentSubjectCreateDto.SubjectId);

        var studentSubject = _mapper.Map<StudentSubject>(studentSubjectCreateDto);

        studentSubject.CreatedDate = DateTime.Now;
        studentSubject.LastUpdatedDate = DateTime.Now;

        await _context.StudentSubjects.AddAsync(studentSubject);
        await _context.SaveChangesAsync();

        var studentSubjectDto = _mapper.Map<StudentSubjectDto>(studentSubject);

        return studentSubjectDto;
    }

    public async Task<StudentSubjectDto> UpdateStudentSubjectAsync(int id, StudentSubjectUpdateDto studentSubjectUpdateDto)
    {
        var studentSubject = await _context.StudentSubjects.FindAsync(id);

        if (studentSubject is null)
            throw new KeyNotFoundException($"StudentSubject with ID {id} was not found.");

        await ValidateStudentAndSubjectAsync(studentSubjectUpdateDto.StudentId, studentSubjectUpdateDto.SubjectId);

        _mapper.Map(studentSubjectUpdateDto, studentSubject);

        studentSubject.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        var studentSubjectDto = _mapper.Map<StudentSubjectDto>(studentSubject);

        return studentSubjectDto;
    }

    public async Task DeleteStudentSubjectAsync(int id)
    {
        var studentSubject = await _context.StudentSubjects.FindAsync(id);

        if (studentSubject is null)
            throw new KeyNotFoundException($"Student subject with ID {id} was not found.");

        studentSubject.IsDeleted = true;

        _context.StudentSubjects.Update(studentSubject);
        await _context.SaveChangesAsync();
    }

    private async Task ValidateStudentAndSubjectAsync(int? studentId, int? subjectId)
    {
        var errors = new List<string>();

        if (studentId.HasValue)
        {
            var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId.Value);
            if (!studentExists)
                errors.Add($"Student with ID {studentId.Value} not found.");
        }

        if (subjectId.HasValue)
        {
            var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == subjectId.Value);
            if (!subjectExists)
                errors.Add($"Subject with ID {subjectId.Value} not found.");
        }

        if (errors.Any())
            throw new KeyNotFoundException(string.Join(" ", errors));
    }
}
