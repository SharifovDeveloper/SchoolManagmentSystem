using AutoMapper;
using Domain.DTOs.Teacher;
using Domain.Entities;
using Domain.Enums;
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
        var teacher = await _context.Teachers.FindAsync(teacherId);

        if (teacher is null)
            throw new KeyNotFoundException($"Teacher with ID {teacherId} was not found.");

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
            throw new KeyNotFoundException($"Teacher with ID {id} not found.");

        var teacherDto = _mapper.Map<TeacherDto>(teacher);

        return teacherDto;
    }

    public async Task<TeacherDto> CreateTeacherAsync(TeacherCreateDto teacherCreateDto)
    {
        await ValidateTeacherDataAsync(teacherCreateDto.CityId, teacherCreateDto.Gender);

        var teacher = _mapper.Map<Teacher>(teacherCreateDto);

        teacher.CreatedDate = DateTime.Now;
        teacher.LastUpdatedDate = DateTime.Now;

        await _context.Teachers.AddAsync(teacher);
        await _context.SaveChangesAsync();

        var teacherDto = _mapper.Map<TeacherDto>(teacher);

        return teacherDto;
    }

    public async Task<TeacherDto> UpdateTeacherAsync(int id, TeacherUpdateDto teacherUpdateDto)
    {
        var teacher = await _context.Teachers.FindAsync(id);

        if (teacher == null)
            throw new KeyNotFoundException($"Teacher with ID {id} not found.");

        await ValidateTeacherDataAsync(teacherUpdateDto.CityId, teacherUpdateDto.Gender);

        _mapper.Map(teacherUpdateDto, teacher);

        teacher.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        var teacherDto = _mapper.Map<TeacherDto>(teacher);
        
        return teacherDto;
    }

    public async Task DeleteTeacherAsync(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);

        if (teacher == null)
            throw new KeyNotFoundException($"Teacher with ID {id} not found.");

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
            throw new KeyNotFoundException(string.Join(" ", errors));
    }
}
