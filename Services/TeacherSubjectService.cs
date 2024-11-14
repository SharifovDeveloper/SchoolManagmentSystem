using AutoMapper;
using Domain.DTOs.TeacherSubject;
using Domain.Entities;
using Domain.Interfaces.Services;
using Domain.Pagniation;
using Domain.ResourceParameters;
using Domain.Responses;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Domain.Helpers;

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

        return teacherSubject == null ? null : _mapper.Map<TeacherSubjectDto>(teacherSubject);
    }
    public async Task<TeacherSubjectDto> CreateTeacherSubjectAsync(TeacherSubjectCreateDto teacherSubjectCreateDto)
    {
        var teacherSubjectEntity = _mapper.Map<TeacherSubject>(teacherSubjectCreateDto);

        teacherSubjectEntity.CreatedDate = DateTime.Now;
        teacherSubjectEntity.LastUpdatedDate = DateTime.Now;

        await _context.TeacherSubjects.AddAsync(teacherSubjectEntity);
        await _context.SaveChangesAsync();

        var teacherSubjectDto = _mapper.Map<TeacherSubjectDto>(teacherSubjectEntity);
        return teacherSubjectDto;
    }


    public async Task<TeacherSubjectDto> UpdateTeacherSubjectAsync(TeacherSubjectUpdateDto teacherSubjectUpdateDto)
    {
        var teacherSubjectEntity = await _context.TeacherSubjects.FindAsync(teacherSubjectUpdateDto.Id);

        if (teacherSubjectEntity == null)
            return null;

        _mapper.Map(teacherSubjectUpdateDto, teacherSubjectEntity);
        teacherSubjectEntity.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();
        return _mapper.Map<TeacherSubjectDto>(teacherSubjectEntity);
    }

    public async Task DeleteTeacherSubjectAsync(int id)
    {
        var teacherSubject = await _context.TeacherSubjects.FirstOrDefaultAsync(ts => ts.Id == id);

        if (teacherSubject == null) return;

        teacherSubject.IsDeleted = true;
        _context.TeacherSubjects.Update(teacherSubject);
        await _context.SaveChangesAsync();
    }
}
