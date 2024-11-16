using AutoMapper;
using Domain.DTOs.TeacherSubject;
using Domain.Entities;
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

    public async Task<GetBaseResponse<TeacherSubjectDto>> GetTeacherSubjectsAsync(TeacherSubjectResourceParameters teacherSubjectResourceParameters)
    {

        if (teacherSubjectResourceParameters.TeacherId.HasValue || teacherSubjectResourceParameters.SubjectId.HasValue)
        {
            await ValidateTeacherAndSubjectAsync(teacherSubjectResourceParameters.TeacherId, teacherSubjectResourceParameters.SubjectId);
        }

        var query = _context.TeacherSubjects
            .AsNoTracking()
            .Include(ts => ts.Teacher)
            .Include(ts => ts.Subject)
            .AsQueryable();

        query = query.ApplyDateFilters(
        teacherSubjectResourceParameters.CreatedDateFrom,
        teacherSubjectResourceParameters.CreatedDateTo,
        teacherSubjectResourceParameters.LastUpdatedDateFrom,
        teacherSubjectResourceParameters.LastUpdatedDateTo);

        query = query.ApplyIsDeletedFilter(teacherSubjectResourceParameters.IsDeleted);

        if (teacherSubjectResourceParameters.TeacherId.HasValue)
        {
            query = query.Where(ts => ts.TeacherId == teacherSubjectResourceParameters.TeacherId.Value);
        }

        if (teacherSubjectResourceParameters.SubjectId.HasValue)
        {
            query = query.Where(ts => ts.SubjectId == teacherSubjectResourceParameters.SubjectId.Value);
        }

        if (!string.IsNullOrEmpty(teacherSubjectResourceParameters.OrderBy))
        {
            query = teacherSubjectResourceParameters.OrderBy.ToLowerInvariant() switch
            {
                "createddate" => query.OrderBy(ts => ts.CreatedDate),
                "createddatedesc" => query.OrderByDescending(ts => ts.CreatedDate),
                _ => query.OrderBy(ts => ts.Id),
            };
        }

        var teacherSubjects = await query.ToPaginatedListAsync(
            teacherSubjectResourceParameters.PageSize,
            teacherSubjectResourceParameters.PageNumber
        );

        var teacherSubjectDtos = _mapper.Map<List<TeacherSubjectDto>>(teacherSubjects);

        var paginatedResult = new PaginatedList<TeacherSubjectDto>(
            teacherSubjectDtos,
            teacherSubjects.TotalCount,
            teacherSubjects.CurrentPage,
            teacherSubjects.PageSize
        );

        return paginatedResult.ToResponse();
    }

    public async Task<TeacherSubjectDto?> GetTeacherSubjectByIdAsync(int id)
    {
        var teacherSubject = await _context.TeacherSubjects
            .AsNoTracking()
            .Include(ts => ts.Teacher)
            .Include(ts => ts.Subject)
            .FirstOrDefaultAsync(ts => ts.Id == id);

        if (teacherSubject == null)
            throw new KeyNotFoundException($"Teacher subject with ID {id} not found.");

        var teacherSubjectDto = _mapper.Map<TeacherSubjectDto>(teacherSubject);

        return teacherSubjectDto;
    }

    public async Task<TeacherSubjectDto> CreateTeacherSubjectAsync(TeacherSubjectCreateDto teacherSubjectCreateDto)
    {
        await ValidateTeacherAndSubjectAsync(teacherSubjectCreateDto.TeacherId, teacherSubjectCreateDto.SubjectId);

        var teacherSubject = _mapper.Map<TeacherSubject>(teacherSubjectCreateDto);

        teacherSubject.CreatedDate = DateTime.Now;
        teacherSubject.LastUpdatedDate = DateTime.Now;

        await _context.TeacherSubjects.AddAsync(teacherSubject);
        await _context.SaveChangesAsync();

        var teacherSubjectDto = _mapper.Map<TeacherSubjectDto>(teacherSubject);

        return teacherSubjectDto;
    }

    public async Task<TeacherSubjectDto> UpdateTeacherSubjectAsync(int id, TeacherSubjectUpdateDto teacherSubjectUpdateDto)
    {
        var teacherSubject = await _context.TeacherSubjects.FindAsync(id);

        if (teacherSubject is null)
            throw new KeyNotFoundException($"Teacher subject with ID {id} not found.");

        await ValidateTeacherAndSubjectAsync(teacherSubjectUpdateDto.TeacherId, teacherSubjectUpdateDto.SubjectId);

        _mapper.Map(teacherSubjectUpdateDto, teacherSubject);
        teacherSubject.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        var teacherSubjectDto = _mapper.Map<TeacherSubjectDto>(teacherSubject);

        return teacherSubjectDto;
    }

    public async Task DeleteTeacherSubjectAsync(int id)
    {
        var teacherSubject = await _context.TeacherSubjects.FindAsync(id);

        if (teacherSubject == null)
            throw new KeyNotFoundException($"Teacher subject with ID {id} not found.");

        teacherSubject.IsDeleted = true;

        _context.TeacherSubjects.Update(teacherSubject);
        await _context.SaveChangesAsync();
    }

    private async Task ValidateTeacherAndSubjectAsync(int? teacherId, int? subjectId)
    {
        var errors = new List<string>();

        if(teacherId.HasValue)
        {
           var teacherExists = await _context.Teachers.AnyAsync(t => t.Id == teacherId);
           if (!teacherExists)
               errors.Add($"Teacher with ID {teacherId} not found.");
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
