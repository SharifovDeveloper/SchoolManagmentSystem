using AutoMapper;
using Domain.DTOs.Student;
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

public class StudentService : IStudentService
{
    private readonly IMapper _mapper;
    private readonly SchoolManagmentDbContext _context;

    public StudentService(IMapper mapper, SchoolManagmentDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetBaseResponse<StudentDto>> GetStudentsAsync(StudentResourceParameters studentResourceParameters)
    {
        var query = _context.Students
            .AsNoTracking()
            .Include(s => s.StudentSubjects)
            .ThenInclude(ss => ss.Subject)
            .Include(s => s.City)
            .AsQueryable();

        query = query.ApplyDateFilters(
        studentResourceParameters.CreatedDateFrom,
        studentResourceParameters.CreatedDateTo,
        studentResourceParameters.LastUpdatedDateFrom,
        studentResourceParameters.LastUpdatedDateTo);

        query = query.ApplyIsDeletedFilter(studentResourceParameters.IsDeleted);

        if (studentResourceParameters.BirthDateFrom.HasValue)
        {
            query = query.Where(s => s.BirthDate >= studentResourceParameters.BirthDateFrom.Value);
        }

        if (studentResourceParameters.BirthDateTo.HasValue)
        {
            query = query.Where(s => s.BirthDate <= studentResourceParameters.BirthDateTo.Value);
        }

        if (studentResourceParameters.Gender.HasValue)
        {
            query = query.Where(s => s.Gender == studentResourceParameters.Gender.Value);
        }

        if (studentResourceParameters.CurrentGradeLevel.HasValue)
        {
            query = query.Where(s => s.CurrentGradeLevel == studentResourceParameters.CurrentGradeLevel.Value);
        }

        if (studentResourceParameters.DepartmentId.HasValue)
        {
            query = query.Where(s => s.DepartmentId == studentResourceParameters.DepartmentId.Value);
        }

        if (studentResourceParameters.CityId.HasValue)
        {
            query = query.Where(s => s.CityId == studentResourceParameters.CityId.Value);
        }

        if (!string.IsNullOrWhiteSpace(studentResourceParameters.SearchString))
        {
            query = query.Where(x => x.Name.Contains(studentResourceParameters.SearchString));
        }

        if (!string.IsNullOrEmpty(studentResourceParameters.OrderBy))
        {
            query = studentResourceParameters.OrderBy.ToLowerInvariant() switch
            {
                "mark" => query.OrderBy(s => s.StudentSubjects.Max(ss => ss.Mark)),
                "markdesc" => query.OrderByDescending(s => s.StudentSubjects.Max(ss => ss.Mark)),
                "grade" => query.OrderBy(s => s.CurrentGradeLevel),
                "gradedesc" => query.OrderByDescending(s => s.CurrentGradeLevel),
                "age" => query.OrderBy(s => DateTime.Now.Year - s.BirthDate.Year),
                "agedesc" => query.OrderByDescending(s => DateTime.Now.Year - s.BirthDate.Year),
                _ => query.OrderBy(s => s.Id),
            };
        }

        var students = await query.ToPaginatedListAsync(studentResourceParameters.PageSize, studentResourceParameters.PageNumber);

        var studentDtos = _mapper.Map<List<StudentDto>>(students);

        var paginatedResult = new PaginatedList<StudentDto>(studentDtos, students.TotalCount, students.CurrentPage, students.PageSize);
        return paginatedResult.ToResponse();
    }

    public async Task<List<string>> GetTop10StudentsBySubjectMarkAsync(int subjectId)
    {
        var subject = await _context.Subjects.FindAsync(subjectId);

        if (subject is null)
            throw new KeyNotFoundException($"Subject with ID {subjectId} was not found.");

        var studentNames = await _context.StudentSubjects
            .Where(ss => ss.SubjectId == subjectId)
            .Include(ss => ss.Student)
            .AsNoTracking()
            .OrderByDescending(ss => ss.Mark)
            .Take(10)
            .Select(ss => ss.Student.Name)
            .ToListAsync();

        return studentNames;
    }

    public async Task<List<string>> GetSubjectsByStudentIdAsync(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);

        if (student is null)
            throw new KeyNotFoundException($"Student with ID {studentId} was not found.");

        var subjectNames = await _context.StudentSubjects
            .AsNoTracking()
            .Where(ts => ts.StudentId == studentId && !ts.IsDeleted)
            .Select(ts => ts.Subject.Name)
            .ToListAsync();

        return subjectNames;
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (student == null)
            throw new KeyNotFoundException($"Student with ID {id} was not found.");

        var studentDto = _mapper.Map<StudentDto>(student);

        return studentDto;
    }

    public async Task<StudentDto> CreateStudentAsync(StudentCreateDto studentCreateDto)
    {
        await ValidateStudentDataAsync(studentCreateDto.CityId, studentCreateDto.DepartmentId, studentCreateDto.Gender);

        var student = _mapper.Map<Student>(studentCreateDto);

        student.CreatedDate = DateTime.Now;
        student.LastUpdatedDate = DateTime.Now;

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        var studentDto = _mapper.Map<StudentDto>(student);

        return studentDto;
    }

    public async Task<StudentDto> UpdateStudentAsync(int id, StudentUpdateDto studentUpdateDto)
    {
        var student = await _context.Students.FindAsync(id);

        if (student is null)
            throw new KeyNotFoundException($"Student with ID {id} was not found.");

        await ValidateStudentDataAsync(studentUpdateDto.CityId, studentUpdateDto.DepartmentId, studentUpdateDto.Gender);

        _mapper.Map(studentUpdateDto, student);

        student.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        var studentDto = _mapper.Map<StudentDto>(student);

        return studentDto;
    }

    public async Task DeleteStudentAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student is null)
            throw new KeyNotFoundException($"Student with ID {id} was not found.");

        student.IsDeleted = true;

        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    private async Task ValidateStudentDataAsync(int cityId, int departmentId, Gender gender)
    {
        var errors = new List<string>();

        var cityExists = await _context.Cities.AnyAsync(c => c.Id == cityId);

        if (!cityExists)
            errors.Add($"City with ID {cityId} not found.");

        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == departmentId);

        if (!departmentExists)
            errors.Add($"Department with ID {departmentId} not found.");

        if (!Enum.IsDefined(typeof(Gender), gender))
            errors.Add($"Invalid gender value: {gender}");

        if (errors.Any())
            throw new KeyNotFoundException(string.Join(" ", errors));
    }

}
