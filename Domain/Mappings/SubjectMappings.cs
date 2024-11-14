using AutoMapper;
using Domain.DTOs.Subject;
using Domain.Entities;

namespace Domain.Mappings;

public class SubjectMappings : Profile
{
    public SubjectMappings()
    {
        CreateMap<Subject, SubjectDto>();
        CreateMap<SubjectCreateDto, Subject>();
        CreateMap<SubjectUpdateDto, Subject>();
    }
}
