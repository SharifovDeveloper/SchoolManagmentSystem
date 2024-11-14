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

        query = query.ApplyDateFilters(
        subjectResourceParameters.CreatedDateFrom,
        subjectResourceParameters.CreatedDateTo,
        subjectResourceParameters.LastUpdatedDateFrom,
        subjectResourceParameters.LastUpdatedDateTo);

        query = query.ApplyIsDeletedFilter(subjectResourceParameters.IsDeleted);

        if (subjectResourceParameters.GradeLevel.HasValue)
        {
            query = query.Where(x => x.GradeLevel == subjectResourceParameters.GradeLevel);
        }

        if (!string.IsNullOrEmpty(subjectResourceParameters.OrderBy))
        {
            query = subjectResourceParameters.OrderBy.ToLowerInvariant() switch
            {
                "name" => query.OrderBy(x => x.Name),
                "namedesc" => query.OrderByDescending(x => x.Name),
                "gradelevel" => query.OrderBy(s => s.GradeLevel),
                "gradeleveldesc" => query.OrderByDescending(s => s.GradeLevel),
                _ => query.OrderBy(x => x.Id),
            };
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

        var subjectDto = _mapper.Map<SubjectDto>(subject);

        return subjectDto;
    }

    public async Task<SubjectDto> CreateSubjectAsync(SubjectCreateDto subjectCreateDto)
    {
        var subjectEntity = _mapper.Map<Subject>(subjectCreateDto);

        subjectEntity.CreatedDate = DateTime.Now;
        subjectEntity.LastUpdatedDate = DateTime.Now;

        await _context.Subjects.AddAsync(subjectEntity);
        await _context.SaveChangesAsync();

        var subjectDto = _mapper.Map<SubjectDto>(subjectEntity);

        return subjectDto;
    }

    public async Task<SubjectDto> UpdateSubjectAsync(SubjectUpdateDto subjectUpdateDto)
    {
        var subjectEntity = await _context.Subjects.FindAsync(subjectUpdateDto.Id);

        if (subjectEntity == null)
        {
            throw new KeyNotFoundException($"Subject with ID {subjectUpdateDto.Id} not found.");
        }

        _mapper.Map(subjectUpdateDto, subjectEntity);

        subjectEntity.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        var subjectDto = _mapper.Map<SubjectDto>(subjectEntity);

        return subjectDto;
    }

    public async Task DeleteSubjectAsync(int id)
    {
        var subject = await _context.Subjects.FirstOrDefaultAsync(x => x.Id == id);

        subject.IsDeleted = true;

        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync();
    }
}
