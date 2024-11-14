namespace Domain.ResourceParameters;

public record SubjectResourceParameters : ResourceParametersBase
{
    public int? GradeLevel { get; set; } 
    public override string OrderBy { get; set; } = "Name";
}
