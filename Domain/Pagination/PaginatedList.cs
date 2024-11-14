using Domain.Responses;

namespace Domain.Pagniation;

public class PaginatedList<T> : List<T> where T : class
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        AddRange(items);
    }

    public GetBaseResponse<T> ToResponse()
        => new()
        {
            Data = this.ToList(),
            HasNextPage = HasNext,
            HasPreviousPage = HasPrevious,
            PageNumber = CurrentPage,
            PageSize = PageSize,
            TotalPages = TotalPages,
            TotalCount = TotalCount
        };
}
