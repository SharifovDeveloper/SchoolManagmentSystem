using Domain.DTOs.Department;
using Domain.ResourceParameters;
using Domain.Responses;

namespace Domain.Interfaces.Services;

public interface IDepartmentService
{
    Task<GetBaseResponse<DepartmentDto>> GetDepartmentsAsync(DepartmentResourceParameters departmentResourceParameters);
    Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
    Task<DepartmentDto> CreateDepartmentAsync(DepartmentCreateDto departmentCreateDto);
    Task<DepartmentDto> UpdateDepartmentAsync(int id, DepartmentUpdateDto departmentUpdateDto);
    Task DeleteDepartmentAsync(int id);
}
