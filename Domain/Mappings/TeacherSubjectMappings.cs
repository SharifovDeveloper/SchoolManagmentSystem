using AutoMapper;
using Domain.DTOs.TeacherSubject;
using Domain.Entities;

namespace Domain.Mappings;

public class TeacherSubjectMappings : Profile
{
    public TeacherSubjectMappings()
    {
        CreateMap<TeacherSubject, TeacherSubjectDto>();
        CreateMap<TeacherSubjectCreateDto, TeacherSubject>();
        CreateMap<TeacherSubjectUpdateDto, TeacherSubject>();
    }
}
