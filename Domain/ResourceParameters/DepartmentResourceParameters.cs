namespace Domain.ResourceParameters;

public record DepartmentResourceParameters : ResourceParametersBase
{
    public int? MinStudentCount { get; set; }
    public int? MaxStudentCount { get; set; }
    public override string OrderBy { get; set; } = "Name";
}