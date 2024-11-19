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

    public async Task<GetBaseResponse<StudentSubjectDto>> GetStudentSubjectsAsync(StudentSubjectResourceParameters resourceParameters)
    {
        if (resourceParameters.StudentId.HasValue || resourceParameters.SubjectId.HasValue)
        {
            await ValidateStudentAndSubjectAsync(resourceParameters.StudentId, resourceParameters.SubjectId);
        }

        var query = _context.StudentSubjects
            .AsNoTracking()
            .Include(ss => ss.Student)
            .Include(ss => ss.Subject)
            .AsQueryable();

        query = ApplyFilters(query, resourceParameters);

        if (!string.IsNullOrWhiteSpace(resourceParameters.SearchString))
        {
            query = query.Where(ss => ss.Student.Name.Contains(resourceParameters.SearchString)
                                    || ss.Subject.Name.Contains(resourceParameters.SearchString));
        }

        if (!string.IsNullOrEmpty(resourceParameters.OrderBy))
        {
            query = ApplyOrdering(query, resourceParameters);
        }

        var paginatedList = await query.ToPaginatedListAsync(resourceParameters.PageSize, resourceParameters.PageNumber);
        var dtoList = _mapper.Map<List<StudentSubjectDto>>(paginatedList);

        var paginatedResult = new PaginatedList<StudentSubjectDto>(dtoList, paginatedList.TotalCount, paginatedList.CurrentPage, paginatedList.PageSize);
        return paginatedResult.ToResponse();
    }

    public async Task<StudentSubjectDto> GetStudentSubjectByIdAsync(int id)
    {
        var studentSubject = await _context.StudentSubjects
            .AsNoTracking()
            .Include(ss => ss.Student)
            .Include(ss => ss.Subject)
            .FirstOrDefaultAsync(ss => ss.Id == id);

        if (studentSubject == null)
            throw new KeyNotFoundException($"StudentSubject with ID {id} was not found.");

        return _mapper.Map<StudentSubjectDto>(studentSubject);
    }

    public async Task<StudentSubjectDto> CreateStudentSubjectAsync(StudentSubjectCreateDto createDto)
    {
        await ValidateStudentAndSubjectAsync(createDto.StudentId, createDto.SubjectId);

        var studentSubject = _mapper.Map<StudentSubject>(createDto);
        studentSubject.CreatedDate = DateTime.Now;
        studentSubject.LastUpdatedDate = DateTime.Now;

        await _context.StudentSubjects.AddAsync(studentSubject);
        await _context.SaveChangesAsync();

        return _mapper.Map<StudentSubjectDto>(studentSubject);
    }

    public async Task<StudentSubjectDto> UpdateStudentSubjectAsync(int id, StudentSubjectUpdateDto updateDto)
    {
        var studentSubject = await _context.StudentSubjects.FindAsync(id);

        if (studentSubject == null)
            throw new KeyNotFoundException($"StudentSubject with ID {id} was not found.");

        await ValidateStudentAndSubjectAsync(updateDto.StudentId, updateDto.SubjectId);

        _mapper.Map(updateDto, studentSubject);
        studentSubject.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        return _mapper.Map<StudentSubjectDto>(studentSubject);
    }

    public async Task DeleteStudentSubjectAsync(int id)
    {
        var studentSubject = await _context.StudentSubjects.FindAsync(id);

        if (studentSubject == null)
            throw new KeyNotFoundException($"StudentSubject with ID {id} was not found.");

        studentSubject.IsDeleted = true;
        _context.StudentSubjects.Update(studentSubject);
        await _context.SaveChangesAsync();
    }

    private async Task ValidateStudentAndSubjectAsync(int? studentId, int? subjectId)
    {
        var errors = new List<string>();
   
        if (studentId.HasValue)
        {
            var studentExists = await _context.Students.AnyAsync(s => s.Id == studentId);
            if(!studentExists)
                errors.Add($"Student with ID {studentId.Value} not found.");

        }
        
        if (subjectId.HasValue)
        {
            var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == subjectId);
            if(!subjectExists)
                errors.Add($"Subject with ID {subjectId.Value} not found.");

        }

        if (errors.Any())
            throw new KeyNotFoundException(string.Join(" ", errors));
    }

    private IQueryable<StudentSubject> ApplyFilters(IQueryable<StudentSubject> query, StudentSubjectResourceParameters parameters)
    {
        query = query.ApplyDateFilters(
            parameters.CreatedDateFrom, parameters.CreatedDateTo,
            parameters.LastUpdatedDateFrom, parameters.LastUpdatedDateTo
        );

        query = query.ApplyIsDeletedFilter(parameters.IsDeleted);

        if (parameters.StudentId.HasValue)
            query = query.Where(ss => ss.StudentId == parameters.StudentId.Value);

        if (parameters.SubjectId.HasValue)
            query = query.Where(ss => ss.SubjectId == parameters.SubjectId.Value);

        if (parameters.MinMark.HasValue)
            query = query.Where(ss => ss.Mark >= parameters.MinMark.Value);

        if (parameters.MaxMark.HasValue)
            query = query.Where(ss => ss.Mark <= parameters.MaxMark.Value);

        return query;
    }

    private IQueryable<StudentSubject> ApplyOrdering(IQueryable<StudentSubject> query, StudentSubjectResourceParameters parameters)
    {
        return parameters.OrderBy.ToLowerInvariant() switch
        {
            "mark" => query.OrderBy(ss => ss.Mark),
            "markdesc" => query.OrderByDescending(ss => ss.Mark),
            "student" => query.OrderBy(ss => ss.Student.Name),
            "studentdesc" => query.OrderByDescending(ss => ss.Student.Name),
            _ => query.OrderBy(ss => ss.Id),
        };
    }
}
