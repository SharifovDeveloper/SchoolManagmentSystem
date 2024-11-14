using AutoMapper;
using Domain.DTOs.Department;
using Domain.Entities;

namespace Domain.Mappings;

public class DepartmentMappings : Profile
{
    public DepartmentMappings()
    {
        CreateMap<Department, DepartmentDto>();
        CreateMap<DepartmentCreateDto, Department>();
        CreateMap<DepartmentUpdateDto, Department>();
    }
}
