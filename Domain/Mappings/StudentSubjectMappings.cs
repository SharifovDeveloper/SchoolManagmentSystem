using AutoMapper;
using Domain.DTOs.StudentSubject;
using Domain.Entities;

namespace Domain.Mappings;

public class StudentSubjectMappings : Profile
{
    public StudentSubjectMappings()
    {
        CreateMap<StudentSubject, StudentSubjectDto>();
        CreateMap<StudentSubjectCreateDto, StudentSubject>();
        CreateMap<StudentSubjectUpdateDto, StudentSubject>();
    }
}
