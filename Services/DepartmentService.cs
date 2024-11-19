using AutoMapper;
using Domain.DTOs.Department;
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

public class DepartmentService : IDepartmentService
{
    private readonly IMapper _mapper;
    private readonly SchoolManagmentDbContext _context;

    public DepartmentService(IMapper mapper, SchoolManagmentDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetBaseResponse<DepartmentDto>> GetDepartmentsAsync(DepartmentResourceParameters departmentResourceParameters)
    {
        var query = _context.Departments
            .AsNoTracking()
            .Include(d => d.Students)
            .AsQueryable();

        query = ApplyFilters(query, departmentResourceParameters);

        if (!string.IsNullOrWhiteSpace(departmentResourceParameters.SearchString))
        {
            query = query.Where(d => d.Name.Contains(departmentResourceParameters.SearchString));
        }

        if (!string.IsNullOrEmpty(departmentResourceParameters.OrderBy))
        {
            query = ApplyOrdering(query, departmentResourceParameters);
        }

        var departments = await query.ToPaginatedListAsync(departmentResourceParameters.PageSize, departmentResourceParameters.PageNumber);
        var departmentDtos = _mapper.Map<List<DepartmentDto>>(departments);

        var paginatedResult = new PaginatedList<DepartmentDto>(departmentDtos, departments.TotalCount, departments.CurrentPage, departments.PageSize);
        return paginatedResult.ToResponse();
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
    {
        var department = await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (department == null)
            throw new EntityNotFoundException($"Department with ID {id} was not found.");

        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(DepartmentCreateDto departmentCreateDto)
    {
        var department = _mapper.Map<Department>(departmentCreateDto);
        department.CreatedDate = DateTime.Now;
        department.LastUpdatedDate = DateTime.Now;

        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();

        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task<DepartmentDto> UpdateDepartmentAsync(int id, DepartmentUpdateDto departmentUpdateDto)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
            throw new EntityNotFoundException($"Department with ID {id} was not found.");

        _mapper.Map(departmentUpdateDto, department);
        department.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task DeleteDepartmentAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);

        if (department == null)
            throw new EntityNotFoundException($"Department with ID {id} was not found.");

        department.IsDeleted = true;

        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
    }

    private IQueryable<Department> ApplyFilters(IQueryable<Department> query, DepartmentResourceParameters parameters)
    {
        query = query.ApplyDateFilters(
            parameters.CreatedDateFrom, parameters.CreatedDateTo,
            parameters.LastUpdatedDateFrom, parameters.LastUpdatedDateTo
        );
        query = query.ApplyIsDeletedFilter(parameters.IsDeleted);

        if (parameters.MinStudentCount.HasValue)
        {
            query = query.Where(d => d.Students.Count >= parameters.MinStudentCount.Value);
        }

        if (parameters.MaxStudentCount.HasValue)
        {
            query = query.Where(d => d.Students.Count <= parameters.MaxStudentCount.Value);
        }

        return query;
    }

    private IQueryable<Department> ApplyOrdering(IQueryable<Department> query, DepartmentResourceParameters parameters)
    {
        return parameters.OrderBy.ToLowerInvariant() switch
        {
            "name" => query.OrderBy(d => d.Name),
            "namedesc" => query.OrderByDescending(d => d.Name),
            "studentscount" => query.OrderBy(d => d.Students.Count),
            "studentscountdesc" => query.OrderByDescending(d => d.Students.Count),
            _ => query.OrderBy(d => d.Id),
        };
    }
}
