using AutoMapper;
using Domain.DTOs.Student;
using Domain.Entities;
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

        var studentDto = _mapper.Map<StudentDto>(student);

        return studentDto;
    }

    public async Task<StudentDto> CreateStudentAsync(StudentCreateDto studentCreateDto)
    {
        var studentEntity = _mapper.Map<Student>(studentCreateDto);

        studentEntity.CreatedDate = DateTime.Now;
        studentEntity.LastUpdatedDate = DateTime.Now;

        await _context.Students.AddAsync(studentEntity);
        await _context.SaveChangesAsync();

        var studentDto = _mapper.Map<StudentDto>(studentEntity);

        return studentDto;
    }

    public async Task<StudentDto> UpdateStudentAsync(StudentUpdateDto studentUpdateDto)
    {
        var studentEntity = await _context.Students.FindAsync(studentUpdateDto.Id);

        if (studentEntity == null)
            return null;

        _mapper.Map(studentUpdateDto, studentEntity);

        studentEntity.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        var studentDto = _mapper.Map<StudentDto>(studentEntity);

        return studentDto;
    }

    public async Task DeleteStudentAsync(int id)
    {
        var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == id);
        if (student == null) return;

        student.IsDeleted = true;

        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }
}
