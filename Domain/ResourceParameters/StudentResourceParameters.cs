using Domain.Enums;

namespace Domain.ResourceParameters;

public record StudentResourceParameters : ResourceParametersBase
{
    public DateTime? BirthDateFrom { get; set; }
    public DateTime? BirthDateTo { get; set; }
    public Gender? Gender { get; set; }
    public int? CurrentGradeLevel { get; set; }
    public int? DepartmentId { get; set; }
    public int? CityId { get; set; }
    public override string OrderBy { get; set; } = "Name";
}
