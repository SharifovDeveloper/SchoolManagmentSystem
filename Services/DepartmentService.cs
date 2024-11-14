using AutoMapper;
using Domain.DTOs.Department;
using Domain.Entities;
using Domain.Interfaces.Services;
using Domain.Pagniation;
using Domain.ResourceParameters;
using Domain.Responses;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Domain.Helpers;

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
            .AsQueryable();

        query = query.ApplyDateFilters(
        departmentResourceParameters.CreatedDateFrom,
        departmentResourceParameters.CreatedDateTo,
        departmentResourceParameters.LastUpdatedDateFrom,
        departmentResourceParameters.LastUpdatedDateTo);

        query = query.ApplyIsDeletedFilter(departmentResourceParameters.IsDeleted);

        if (departmentResourceParameters.MinStudentCount.HasValue)
        {
            query = query.Where(x => x.Students.Count >= departmentResourceParameters.MinStudentCount.Value);
        }

        if (departmentResourceParameters.MaxStudentCount.HasValue)
        {
            query = query.Where(x => x.Students.Count <= departmentResourceParameters.MaxStudentCount.Value);
        }

        if (!string.IsNullOrWhiteSpace(departmentResourceParameters.SearchString))
        {
            query = query.Where(x => x.Name.Contains(departmentResourceParameters.SearchString));
        }

        if (!string.IsNullOrEmpty(departmentResourceParameters.OrderBy))
        {
            query = departmentResourceParameters.OrderBy.ToLowerInvariant() switch
            {
                "name" => query.OrderBy(x => x.Name),
                "namedesc" => query.OrderByDescending(x => x.Name),
                _ => query.OrderBy(x => x.Id),
            };
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
            .FirstOrDefaultAsync(x => x.Id == id);

        var departmentDto = _mapper.Map<DepartmentDto>(department);

        return departmentDto;
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(DepartmentCreateDto departmentCreateDto)
    {
        var departmentEntity = _mapper.Map<Department>(departmentCreateDto);

        departmentEntity.CreatedDate = DateTime.Now;
        departmentEntity.LastUpdatedDate = DateTime.Now;

        await _context.Departments.AddAsync(departmentEntity);
        await _context.SaveChangesAsync();

        var departmentDto = _mapper.Map<DepartmentDto>(departmentEntity);

        return departmentDto;
    }

    public async Task<DepartmentDto> UpdateDepartmentAsync(DepartmentUpdateDto departmentUpdateDto)
    {
        var departmentEntity = await _context.Departments.FindAsync(departmentUpdateDto.Id);

        _mapper.Map(departmentUpdateDto, departmentEntity);

        departmentEntity.LastUpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        var departmentDto = _mapper.Map<DepartmentDto>(departmentEntity);
        return departmentDto;
    }

    public async Task DeleteDepartmentAsync(int id)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == id);
        department.IsDeleted = true;

        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
    }
}
