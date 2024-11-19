using AutoMapper;
using Domain.DTOs.TeacherSubject;
using Domain.Entities;
using Domain.Exeptions;
using Domain.Helpers;
using Domain.Interfaces.Services;
using Domain.Pagniation;
using Domain.ResourceParameters;
using Domain.Responses;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class TeacherSubjectService : ITeacherSubjectService
{
    private readonly IMapper _mapper;
    private readonly SchoolManagmentDbContext _context;

    public TeacherSubjectService(IMapper mapper, SchoolManagmentDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetBaseResponse<TeacherSubjectDto>> GetTeacherSubjectsAsync(TeacherSubjectResourceParameters resourceParameters)
    {
        if (resourceParameters.TeacherId.HasValue || resourceParameters.SubjectId.HasValue)
        {
            await ValidateTeacherAndSubjectAsync(resourceParameters.TeacherId, resourceParameters.SubjectId);
        }

        var query = _context.TeacherSubjects
            .AsNoTracking()
            .Include(ts => ts.Teacher)
            .Include(ts => ts.Subject)
            .AsQueryable();

        query = ApplyFilters(query, resourceParameters);

        if (!string.IsNullOrWhiteSpace(resourceParameters.SearchString))
        {
            query = query.Where(ts => ts.Teacher.Name.Contains(resourceParameters.SearchString)
                                    || ts.Subject.Name.Contains(resourceParameters.SearchString));
        }

        if (!string.IsNullOrEmpty(resourceParameters.OrderBy))
        {
            query = ApplyOrdering(query, resourceParameters);
        }

        var paginatedList = await query.ToPaginatedListAsync(resourceParameters.PageSize, resourceParameters.PageNumber);
        var dtoList = _mapper.Map<List<TeacherSubjectDto>>(paginatedList);

        var paginatedResult = new PaginatedList<TeacherSubjectDto>(dtoList, paginatedList.TotalCount, paginatedList.CurrentPage, paginatedList.PageSize);
        return paginatedResult.ToResponse();
    }

    public async Task<TeacherSubjectDto> GetTeacherSubjectByIdAsync(int id)
    {
        var teacherSubject = await _context.TeacherSubjects
            .AsNoTracking()
            .Include(ts => ts.Teacher)
            .Include(ts => ts.Subject)
            .FirstOrDefaultAsync(ts => ts.Id == id);

        if (teacherSubject == null)
            throw new EntityNotFoundException($"TeacherSubject with ID {id} was not found.");

        return _mapper.Map<TeacherSubjectDto>(teacherSubject);
    }

    public async Task<TeacherSubjectDto> CreateTeacherSubjectAsync(TeacherSubjectCreateDto createDto)
    {
        await ValidateTeacherAndSubjectAsync(createDto.TeacherId, createDto.SubjectId);

        var teacherSubject = _mapper.Map<TeacherSubject>(createDto);
        teacherSubject.CreatedDate = DateTime.Now;
        teacherSubject.LastUpdatedDate = DateTime.Now;

        await _context.TeacherSubjects.AddAsync(teacherSubject);
        await _context.SaveChangesAsync();

        return _mapper.Map<TeacherSubjectDto>(teacherSubject);
    }

    public async Task<TeacherSubjectDto> UpdateTeacherSubjectAsync(int id, TeacherSubjectUpdateDto updateDto)
    {
        var teacherSubject = await _context.TeacherSubjects.FindAsync(id);

        if (teacherSubject == null)
            throw new EntityNotFoundException($"TeacherSubject with ID {id} was not found.");

        await ValidateTeacherAndSubjectAsync(updateDto.TeacherId, updateDto.SubjectId);

        _mapper.Map(updateDto, teacherSubject);
        teacherSubject.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        return _mapper.Map<TeacherSubjectDto>(teacherSubject);
    }

    public async Task DeleteTeacherSubjectAsync(int id)
    {
        var teacherSubject = await _context.TeacherSubjects.FindAsync(id);

        if (teacherSubject == null)
            throw new EntityNotFoundException($"TeacherSubject with ID {id} was not found.");

        teacherSubject.IsDeleted = true;
        _context.TeacherSubjects.Update(teacherSubject);
        await _context.SaveChangesAsync();
    }

    private async Task ValidateTeacherAndSubjectAsync(int? teacherId, int? subjectId)
    {
        var errors = new List<string>();

        if (teacherId.HasValue)
        {
            var teacherExists = await _context.Teachers.AnyAsync(t => t.Id == teacherId);
            if (!teacherExists)
                errors.Add($"Teacher with ID {teacherId.Value} not found.");
        }

        if (subjectId.HasValue)
        {
            var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == subjectId.Value);
            if (!subjectExists)
                errors.Add($"Subject with ID {subjectId.Value} not found.");
        }

        if (errors.Any())
            throw new EntityNotFoundException(string.Join(" ", errors));
    }

    private IQueryable<TeacherSubject> ApplyFilters(IQueryable<TeacherSubject> query, TeacherSubjectResourceParameters parameters)
    {
        query = query.ApplyDateFilters(
            parameters.CreatedDateFrom, parameters.CreatedDateTo,
            parameters.LastUpdatedDateFrom, parameters.LastUpdatedDateTo
        );

        query = query.ApplyIsDeletedFilter(parameters.IsDeleted);

        if (parameters.TeacherId.HasValue)
            query = query.Where(ts => ts.TeacherId == parameters.TeacherId.Value);

        if (parameters.SubjectId.HasValue)
            query = query.Where(ts => ts.SubjectId == parameters.SubjectId.Value);

        return query;
    }

    private IQueryable<TeacherSubject> ApplyOrdering(IQueryable<TeacherSubject> query, TeacherSubjectResourceParameters parameters)
    {
        return parameters.OrderBy.ToLowerInvariant() switch
        {
            "createddate" => query.OrderBy(ts => ts.CreatedDate),
            "createddatedesc" => query.OrderByDescending(ts => ts.CreatedDate),
            _ => query.OrderBy(ts => ts.Id),
        };
    }
}
