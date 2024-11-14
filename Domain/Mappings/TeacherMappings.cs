using AutoMapper;
using Domain.DTOs.Teacher;
using Domain.Entities;

namespace Domain.Mappings;

public class TeacherMappings : Profile
{
    public TeacherMappings()
    {
        CreateMap<Teacher, TeacherDto>();
        CreateMap<TeacherCreateDto, Teacher>();
        CreateMap<TeacherUpdateDto, Teacher>();
    }
}
