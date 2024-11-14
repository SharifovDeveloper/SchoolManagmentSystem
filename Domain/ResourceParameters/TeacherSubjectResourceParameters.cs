using Swashbuckle.AspNetCore.Annotations;

namespace Domain.ResourceParameters;

public record TeacherSubjectResourceParameters : ResourceParametersBase
{
    public int? TeacherId { get; set; } 
    public int? SubjectId { get; set; } 
    public override string OrderBy { get; set; } = "CreatedDate";

    [SwaggerIgnore]
    public override string? SearchString { get; set; }
}
