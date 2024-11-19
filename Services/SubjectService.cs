using AutoMapper;
using Domain.DTOs.Subject;
using Domain.Entities;
using Domain.Helpers;
using Domain.Interfaces.Services;
using Domain.Pagniation;
using Domain.ResourceParameters;
using Domain.Responses;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class SubjectService : ISubjectService
{
    private readonly IMapper _mapper;
    private readonly SchoolManagmentDbContext _context;

    public SubjectService(IMapper mapper, SchoolManagmentDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetBaseResponse<SubjectDto>> GetSubjectsAsync(SubjectResourceParameters subjectResourceParameters)
    {
        var query = _context.Subjects
            .AsNoTracking()
            .AsQueryable();

        query = ApplyFilters(query, subjectResourceParameters);

        if (!string.IsNullOrEmpty(subjectResourceParameters.OrderBy))
        {
            query = ApplyOrdering(query, subjectResourceParameters);
        }

        var subjects = await query.ToPaginatedListAsync(subjectResourceParameters.PageSize, subjectResourceParameters.PageNumber);
        var subjectDtos = _mapper.Map<List<SubjectDto>>(subjects);

        var paginatedResult = new PaginatedList<SubjectDto>(subjectDtos, subjects.TotalCount, subjects.CurrentPage, subjects.PageSize);
        return paginatedResult.ToResponse();
    }

    public async Task<SubjectDto?> GetSubjectByIdAsync(int id)
    {
        var subject = await _context.Subjects
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (subject == null)
            throw new KeyNotFoundException($"Subject with ID {id} not found.");

        return _mapper.Map<SubjectDto>(subject);
    }

    public async Task<SubjectDto> CreateSubjectAsync(SubjectCreateDto subjectCreateDto)
    {
        var subject = _mapper.Map<Subject>(subjectCreateDto);

        subject.CreatedDate = DateTime.Now;
        subject.LastUpdatedDate = DateTime.Now;

        await _context.Subjects.AddAsync(subject);
        await _context.SaveChangesAsync();

        return _mapper.Map<SubjectDto>(subject);
    }

    public async Task<SubjectDto> UpdateSubjectAsync(int id, SubjectUpdateDto subjectUpdateDto)
    {
        var subject = await _context.Subjects.FindAsync(id);

        if (subject == null)
            throw new KeyNotFoundException($"Subject with ID {id} not found.");

        _mapper.Map(subjectUpdateDto, subject);

        subject.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        return _mapper.Map<SubjectDto>(subject);
    }

    public async Task DeleteSubjectAsync(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);

        if (subject == null)
            throw new KeyNotFoundException($"Subject with ID {id} not found.");

        subject.IsDeleted = true;

        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync();
    }

    private IQueryable<Subject> ApplyFilters(IQueryable<Subject> query, SubjectResourceParameters parameters)
    {
        query = query.ApplyDateFilters(
            parameters.CreatedDateFrom, parameters.CreatedDateTo,
            parameters.LastUpdatedDateFrom, parameters.LastUpdatedDateTo
        );
        query = query.ApplyIsDeletedFilter(parameters.IsDeleted);

        if (parameters.GradeLevel.HasValue)
            query = query.Where(s => s.GradeLevel == parameters.GradeLevel.Value);

        return query;
    }

    private IQueryable<Subject> ApplyOrdering(IQueryable<Subject> query, SubjectResourceParameters parameters)
    {
        return parameters.OrderBy.ToLowerInvariant() switch
        {
            "name" => query.OrderBy(s => s.Name),
            "namedesc" => query.OrderByDescending(s => s.Name),
            "gradelevel" => query.OrderBy(s => s.GradeLevel),
            "gradeleveldesc" => query.OrderByDescending(s => s.GradeLevel),
            _ => query.OrderBy(s => s.Id),
        };
    }
}
