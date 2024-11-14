namespace Domain.ResourceParameters;

public record CityResourceParameters : ResourceParametersBase
{
    public override string OrderBy { get; set; } = "Name";
}