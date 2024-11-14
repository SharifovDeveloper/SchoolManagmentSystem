using Swashbuckle.AspNetCore.Annotations;

namespace Domain.ResourceParameters;

public abstract record ResourceParametersBase
{
    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }

    public DateTime? LastUpdatedDateFrom { get; set; }
    public DateTime? LastUpdatedDateTo { get; set; }
    public bool? IsDeleted { get; set; }
    protected virtual int MaxPageSize { get; set; } = 25;

    public virtual string? SearchString { get; set; }

    public abstract string OrderBy { get; set; }
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 15;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
