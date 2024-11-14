using AutoMapper;
using Domain.DTOs.Teacher;
using Domain.Entities;
using Domain.Interfaces.Services;
using Domain.Pagniation;
using Domain.ResourceParameters;
using Domain.Responses;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Domain.Helpers;

namespace Services;

public class TeacherService : ITeacherService
{
    private readonly IMapper _mapper;
    private readonly SchoolManagmentDbContext _context;

    public TeacherService(IMapper mapper, SchoolManagmentDbContext context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<GetBaseResponse<TeacherDto>> GetTeachersAsync(TeacherResourceParameters teacherResourceParameters)
    {
        var query = _context.Teachers
            .AsNoTracking()
            .Include(t => t.TeacherSubjects)
            .Include(t => t.City)
            .AsQueryable();

        query = query.ApplyDateFilters(
        teacherResourceParameters.CreatedDateFrom,
        teacherResourceParameters.CreatedDateTo,
        teacherResourceParameters.LastUpdatedDateFrom,
        teacherResourceParameters.LastUpdatedDateTo);

        query = query.ApplyIsDeletedFilter(teacherResourceParameters.IsDeleted);

        if (teacherResourceParameters.BirthDateFrom.HasValue)
        {
            query = query.Where(t => t.BirthDate >= teacherResourceParameters.BirthDateFrom.Value);
        }

        if (teacherResourceParameters.BirthDateTo.HasValue)
        {
            query = query.Where(t => t.BirthDate <= teacherResourceParameters.BirthDateTo.Value);
        }

        if (teacherResourceParameters.Gender.HasValue)
        {
            query = query.Where(t => t.Gender == teacherResourceParameters.Gender.Value);
        }

        if (teacherResourceParameters.CityId.HasValue)
        {
            query = query.Where(t => t.CityId == teacherResourceParameters.CityId.Value);
        }      

        if (!string.IsNullOrEmpty(teacherResourceParameters.OrderBy))
        {
            query = teacherResourceParameters.OrderBy.ToLowerInvariant() switch
            {
                "name" => query.OrderBy(t => t.Name),
                "namedesc" => query.OrderByDescending(t => t.Name),
                "birthdate" => query.OrderBy(t => t.BirthDate),
                "birthdatedesc" => query.OrderByDescending(t => t.BirthDate),
                _ => query.OrderBy(t => t.Id),
            };
        }

        var teachers = await query.ToPaginatedListAsync(teacherResourceParameters.PageSize, teacherResourceParameters.PageNumber);

        var teacherDtos = _mapper.Map<List<TeacherDto>>(teachers);

        var paginatedResult = new PaginatedList<TeacherDto>(teacherDtos, teachers.TotalCount, teachers.CurrentPage, teachers.PageSize);

        return paginatedResult.ToResponse();
    }
    public async Task<List<string>> GetTeachersForTop5StudentsWithHighestMarksAsync()
    {
        var topStudents = await _context.StudentSubjects
            .AsNoTracking()
            .GroupBy(ss => ss.StudentId)
            .Select(g => new
            {
                StudentId = g.Key,
                AverageMark = g.Average(ss => ss.Mark)
            })
            .OrderByDescending(s => s.AverageMark)
            .Take(5)
            .Select(s => s.StudentId)
            .ToListAsync();

        var teacherNames = await _context.TeacherSubjects
            .AsNoTracking()
            .Where(ts => ts.Subject.StudentSubjects.Any(ss => topStudents.Contains(ss.StudentId)))
            .Select(ts => ts.Teacher.Name)
            .Distinct()
            .ToListAsync();

        return teacherNames;
    }
    public async Task<List<string>> GetTeachersForTop5StudentsWithLowestMarksAsync()
    {
        var bottomStudents = await _context.StudentSubjects
            .AsNoTracking()
            .GroupBy(ss => ss.StudentId)
            .Select(g => new
            {
                StudentId = g.Key,
                AverageMark = g.Average(ss => ss.Mark)
            })
            .OrderBy(s => s.AverageMark)
            .Take(5)
            .Select(s => s.StudentId)
            .ToListAsync();

        var teacherNames = await _context.TeacherSubjects
            .AsNoTracking()
            .Where(ts => ts.Subject.StudentSubjects.Any(ss => bottomStudents.Contains(ss.StudentId)))
            .Select(ts => ts.Teacher.Name)
            .Distinct()
            .ToListAsync();

        return teacherNames;
    }
    public async Task<List<string>> GetSubjectsByTeacherIdAsync(int teacherId)
    {
        var subjectNames = await _context.TeacherSubjects
            .AsNoTracking()
            .Where(ts => ts.TeacherId == teacherId && !ts.IsDeleted)
            .Select(ts => ts.Subject.Name)
            .ToListAsync();

        return subjectNames;
    }
    public async Task<TeacherDto?> GetTeacherByIdAsync(int id)
    {
        var teacher = await _context.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        var teacherDto = _mapper.Map<TeacherDto>(teacher);

        return teacherDto;
    }

    public async Task<TeacherDto> CreateTeacherAsync(TeacherCreateDto teacherCreateDto)
    {
        var teacherEntity = _mapper.Map<Teacher>(teacherCreateDto);

        teacherEntity.CreatedDate = DateTime.Now;
        teacherEntity.LastUpdatedDate = DateTime.Now;

        await _context.Teachers.AddAsync(teacherEntity);
        await _context.SaveChangesAsync();

        var teacherDto = _mapper.Map<TeacherDto>(teacherEntity);

        return teacherDto;
    }

    public async Task<TeacherDto> UpdateTeacherAsync(TeacherUpdateDto teacherUpdateDto)
    {
        var teacherEntity = await _context.Teachers.FindAsync(teacherUpdateDto.Id);

        _mapper.Map(teacherUpdateDto, teacherEntity);

        teacherEntity.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        var teacherDto = _mapper.Map<TeacherDto>(teacherEntity);
        return teacherDto;
    }

    public async Task DeleteTeacherAsync(int id)
    {
        var teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.Id == id);
        teacher.IsDeleted = true;

        _context.Teachers.Update(teacher);
        await _context.SaveChangesAsync();
    }
}
