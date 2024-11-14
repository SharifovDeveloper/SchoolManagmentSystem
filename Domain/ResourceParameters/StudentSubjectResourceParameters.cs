using Swashbuckle.AspNetCore.Annotations;

namespace Domain.ResourceParameters;

public record StudentSubjectResourceParameters : ResourceParametersBase
{
    public int? SubjectId { get; set; }
    public int? StudentId { get; set; }
    public int? MinMark { get; set; }
    public int? MaxMark { get; set; }
    public override string OrderBy { get; set; } = "markdesc";

    [SwaggerIgnore]
    public override string? SearchString { get; set; }
}
