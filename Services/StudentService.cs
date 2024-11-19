using AutoMapper;
using Domain.DTOs.Student;
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

        query = ApplyFilters(query, studentResourceParameters);

        if (!string.IsNullOrWhiteSpace(studentResourceParameters.SearchString))
        {
            query = query.Where(x => x.Name.Contains(studentResourceParameters.SearchString));
        }

        if (!string.IsNullOrEmpty(studentResourceParameters.OrderBy))
        {
            query = ApplyOrdering(query, studentResourceParameters);
        }

        var students = await query.ToPaginatedListAsync(studentResourceParameters.PageSize, studentResourceParameters.PageNumber);
        var studentDtos = _mapper.Map<List<StudentDto>>(students);

        var paginatedResult = new PaginatedList<StudentDto>(studentDtos, students.TotalCount, students.CurrentPage, students.PageSize);
        return paginatedResult.ToResponse();
    }

    public async Task<List<string>> GetTop10StudentsBySubjectMarkAsync(int subjectId)
    {
        var subject = await _context.Subjects.FindAsync(subjectId);

        if (subject == null)
            throw new EntityNotFoundException($"Subject with ID {subjectId} was not found.");

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

        if (student == null)
            throw new EntityNotFoundException($"Student with ID {studentId} was not found.");

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
            throw new EntityNotFoundException($"Student with ID {id} was not found.");

        return _mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto> CreateStudentAsync(StudentCreateDto studentCreateDto)
    {
        await ValidateStudentDataAsync(studentCreateDto.CityId, studentCreateDto.DepartmentId, studentCreateDto.Gender);

        var student = _mapper.Map<Student>(studentCreateDto);
        student.CreatedDate = DateTime.Now;
        student.LastUpdatedDate = DateTime.Now;

        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();

        return _mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto> UpdateStudentAsync(int id, StudentUpdateDto studentUpdateDto)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
            throw new EntityNotFoundException($"Student with ID {id} was not found.");

        await ValidateStudentDataAsync(studentUpdateDto.CityId, studentUpdateDto.DepartmentId, studentUpdateDto.Gender);

        _mapper.Map(studentUpdateDto, student);
        student.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        return _mapper.Map<StudentDto>(student);
    }

    public async Task DeleteStudentAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
            throw new EntityNotFoundException($"Student with ID {id} was not found.");

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
            throw new EntityNotFoundException(string.Join(" ", errors));
    }

    private IQueryable<Student> ApplyFilters(IQueryable<Student> query, StudentResourceParameters parameters)
    {
        query = query.ApplyDateFilters(
            parameters.CreatedDateFrom, parameters.CreatedDateTo,
            parameters.LastUpdatedDateFrom, parameters.LastUpdatedDateTo
        );
        query = query.ApplyIsDeletedFilter(parameters.IsDeleted);

        if (parameters.BirthDateFrom.HasValue)
            query = query.Where(s => s.BirthDate >= parameters.BirthDateFrom.Value);

        if (parameters.BirthDateTo.HasValue)
            query = query.Where(s => s.BirthDate <= parameters.BirthDateTo.Value);

        if (parameters.Gender.HasValue)
            query = query.Where(s => s.Gender == parameters.Gender.Value);

        if (parameters.CurrentGradeLevel.HasValue)
            query = query.Where(s => s.CurrentGradeLevel == parameters.CurrentGradeLevel.Value);

        if (parameters.DepartmentId.HasValue)
            query = query.Where(s => s.DepartmentId == parameters.DepartmentId.Value);

        if (parameters.CityId.HasValue)
            query = query.Where(s => s.CityId == parameters.CityId.Value);

        return query;
    }

    private IQueryable<Student> ApplyOrdering(IQueryable<Student> query, StudentResourceParameters parameters)
    {
        return parameters.OrderBy.ToLowerInvariant() switch
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
}
