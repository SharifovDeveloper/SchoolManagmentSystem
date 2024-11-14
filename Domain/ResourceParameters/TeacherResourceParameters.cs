using Domain.Enums;

namespace Domain.ResourceParameters;

public record TeacherResourceParameters : ResourceParametersBase
{
    public DateTime? BirthDateFrom { get; set; }
    public DateTime? BirthDateTo { get; set; }
    public Gender? Gender { get; set; }
    public int? CityId { get; set; }
    public override string OrderBy { get; set; } = "Name";
}
