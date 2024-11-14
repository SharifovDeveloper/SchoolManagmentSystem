using AutoMapper;
using Domain.DTOs.StudentSubject;
using Domain.Entities;
using Domain.Interfaces.Services;
using Domain.Pagniation;
using Domain.ResourceParameters;
using Domain.Responses;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Domain.Helpers;

namespace Services;

public class StudentSubjectService : IStudentSubjectService
{
    private readonly IMapper _mapper;
    private readonly SchoolManagmentDbContext _context;

    public StudentSubjectService(IMapper mapper, SchoolManagmentDbContext context)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<GetBaseResponse<StudentSubjectDto>> GetStudentSubjectsAsync(StudentSubjectResourceParameters studentSubjectResourceParameters)
    {
        var query = _context.StudentSubjects
            .AsNoTracking()
            .Include(ss => ss.Student)
            .Include(ss => ss.Subject)
            .AsQueryable();

        query = query.ApplyDateFilters(
        studentSubjectResourceParameters.CreatedDateFrom,
        studentSubjectResourceParameters.CreatedDateTo,
        studentSubjectResourceParameters.LastUpdatedDateFrom,
        studentSubjectResourceParameters.LastUpdatedDateTo);

        query = query.ApplyIsDeletedFilter(studentSubjectResourceParameters.IsDeleted);

        if (studentSubjectResourceParameters.StudentId.HasValue)
        {
            query = query.Where(ss => ss.StudentId == studentSubjectResourceParameters.StudentId.Value);
        }

        if (studentSubjectResourceParameters.SubjectId.HasValue)
        {
            query = query.Where(ss => ss.SubjectId == studentSubjectResourceParameters.SubjectId.Value);
        }

        if (studentSubjectResourceParameters.MinMark.HasValue)
        {
            query = query.Where(ss => ss.Mark >= studentSubjectResourceParameters.MinMark.Value);
        }

        if (studentSubjectResourceParameters.MaxMark.HasValue)
        {
            query = query.Where(ss => ss.Mark <= studentSubjectResourceParameters.MaxMark.Value);
        }

        if (!string.IsNullOrEmpty(studentSubjectResourceParameters.OrderBy))
        {
            query = studentSubjectResourceParameters.OrderBy.ToLowerInvariant() switch
            {
                "mark" => query.OrderBy(ss => ss.Mark),
                "markdesc" => query.OrderByDescending(ss => ss.Mark),
                _ => query.OrderBy(ss => ss.Student.Id),
            };
        }

        var studentSubjects = await query.ToPaginatedListAsync(studentSubjectResourceParameters.PageSize, studentSubjectResourceParameters.PageNumber);

        var studentSubjectDtos = _mapper.Map<List<StudentSubjectDto>>(studentSubjects);

        var paginatedResult = new PaginatedList<StudentSubjectDto>(studentSubjectDtos, studentSubjects.TotalCount, studentSubjects.CurrentPage, studentSubjects.PageSize);
        return paginatedResult.ToResponse();
    }

    public async Task<StudentSubjectDto> GetStudentSubjectByIdAsync(int id)
    {
        var studentSubject = await _context.StudentSubjects
            .AsNoTracking()
            .Include(ss => ss.Student)
            .Include(ss => ss.Subject)
            .FirstOrDefaultAsync(ss => ss.Id == id);

        if (studentSubject == null)
            return null;

        return _mapper.Map<StudentSubjectDto>(studentSubject);
    }

    public async Task<StudentSubjectDto> CreateStudentSubjectAsync(StudentSubjectCreateDto studentSubjectCreateDto)
    {
        var studentSubjectEntity = _mapper.Map<StudentSubject>(studentSubjectCreateDto);

        studentSubjectEntity.CreatedDate = DateTime.Now;
        studentSubjectEntity.LastUpdatedDate = DateTime.Now;

        await _context.StudentSubjects.AddAsync(studentSubjectEntity);
        await _context.SaveChangesAsync();

        var studentSubjectDto = _mapper.Map<StudentSubjectDto>(studentSubjectEntity);
        return studentSubjectDto;
    }

    public async Task<StudentSubjectDto> UpdateStudentSubjectAsync(StudentSubjectUpdateDto studentSubjectUpdateDto)
    {
        var studentSubjectEntity = await _context.StudentSubjects.FindAsync(studentSubjectUpdateDto.Id);

        _mapper.Map(studentSubjectUpdateDto, studentSubjectEntity);

        studentSubjectEntity.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        var studentSubjectDto = _mapper.Map<StudentSubjectDto>(studentSubjectEntity);
        return studentSubjectDto;
    }

    public async Task DeleteStudentSubjectAsync(int id)
    {
        var studentSubject = await _context.StudentSubjects.FirstOrDefaultAsync(ss => ss.Id == id);

        studentSubject.IsDeleted = true;

        _context.StudentSubjects.Update(studentSubject);
        await _context.SaveChangesAsync();
    }
}
