using AutoMapper;
using Domain.DTOs.Student;
using Domain.Entities;

namespace Domain.Mappings;

public class StudentMappings : Profile
{
    public StudentMappings()
    {
        CreateMap<Student, StudentDto>();
        CreateMap<StudentCreateDto, Student>();
        CreateMap<StudentUpdateDto, Student>();
    }
}
