using AutoMapper;
using Domain.DTOs.Teacher;
using Domain.Entities;
using Domain.Enums;
using Domain.Exeptions;
using Domain.Helpers;
using Domain.Interfaces.Services;
using Domain.Pagniation;
using Domain.ResourceParameters;
using Domain.Responses;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class TeacherService : ITeacherService
{
    private readonly IMapper _mapper;
    private readonly SchoolManagmentDbContext _context;

    public TeacherService(IMapper mapper, SchoolManagmentDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetBaseResponse<TeacherDto>> GetTeachersAsync(TeacherResourceParameters teacherResourceParameters)
    {
        var query = _context.Teachers
            .AsNoTracking()
            .Include(t => t.TeacherSubjects)
            .Include(t => t.City)
            .AsQueryable();

        query = ApplyFilters(query, teacherResourceParameters);

        if (!string.IsNullOrEmpty(teacherResourceParameters.OrderBy))
        {
            query = ApplyOrdering(query, teacherResourceParameters);
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
        var teacher = await _context.Teachers.FindAsync(teacherId);

        if (teacher == null)
            throw new EntityNotFoundException($"Teacher with ID {teacherId} was not found.");

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

        if (teacher == null)
            throw new EntityNotFoundException($"Teacher with ID {id} was not found.");

        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> CreateTeacherAsync(TeacherCreateDto teacherCreateDto)
    {
        await ValidateTeacherDataAsync(teacherCreateDto.CityId, teacherCreateDto.Gender);

        var teacher = _mapper.Map<Teacher>(teacherCreateDto);
        teacher.CreatedDate = DateTime.Now;
        teacher.LastUpdatedDate = DateTime.Now;

        await _context.Teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();

        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> UpdateTeacherAsync(int id, TeacherUpdateDto teacherUpdateDto)
    {
        var teacher = await _context.Teachers.FindAsync(id);

        if (teacher == null)
            throw new EntityNotFoundException($"Teacher with ID {id} was not found.");

        await ValidateTeacherDataAsync(teacherUpdateDto.CityId, teacherUpdateDto.Gender);

        _mapper.Map(teacherUpdateDto, teacher);
        teacher.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task DeleteTeacherAsync(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);

        if (teacher == null)
            throw new EntityNotFoundException($"Teacher with ID {id} was not found.");

        teacher.IsDeleted = true;

        _context.Teachers.Update(teacher);
        await _context.SaveChangesAsync();
    }

    private async Task ValidateTeacherDataAsync(int cityId, Gender gender)
    {
        var errors = new List<string>();

        var cityExists = await _context.Cities.AnyAsync(c => c.Id == cityId);

        if (!cityExists)
            errors.Add($"City with ID {cityId} not found.");

        if (!Enum.IsDefined(typeof(Gender), gender))
            errors.Add($"Invalid gender value: {gender}");

        if (errors.Any())
            throw new EntityNotFoundException(string.Join(" ", errors));
    }

    private IQueryable<Teacher> ApplyFilters(IQueryable<Teacher> query, TeacherResourceParameters parameters)
    {
        query = query.ApplyDateFilters(
            parameters.CreatedDateFrom, parameters.CreatedDateTo,
            parameters.LastUpdatedDateFrom, parameters.LastUpdatedDateTo
        );
        query = query.ApplyIsDeletedFilter(parameters.IsDeleted);

        if (parameters.BirthDateFrom.HasValue)
            query = query.Where(t => t.BirthDate >= parameters.BirthDateFrom.Value);

        if (parameters.BirthDateTo.HasValue)
            query = query.Where(t => t.BirthDate <= parameters.BirthDateTo.Value);

        if (parameters.Gender.HasValue)
            query = query.Where(t => t.Gender == parameters.Gender.Value);

        if (parameters.CityId.HasValue)
            query = query.Where(t => t.CityId == parameters.CityId.Value);

        return query;
    }

    private IQueryable<Teacher> ApplyOrdering(IQueryable<Teacher> query, TeacherResourceParameters parameters)
    {
        return parameters.OrderBy.ToLowerInvariant() switch
        {
            "name" => query.OrderBy(t => t.Name),
            "namedesc" => query.OrderByDescending(t => t.Name),
            "birthdate" => query.OrderBy(t => t.BirthDate),
            "birthdatedesc" => query.OrderByDescending(t => t.BirthDate),
            _ => query.OrderBy(t => t.Id),
        };
    }
}
