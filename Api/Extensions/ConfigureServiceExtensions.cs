using Domain.Interfaces.Services;
using Services;

namespace Domain.Extensions;

public static class ConfigureServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IStudentSubjectService, StudentSubjectService>();
        services.AddScoped<ITeacherSubjectService, TeacherSubjectService>();
      
        return services;
    }
}
