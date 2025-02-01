namespace FragranceRecommendation.DTOs;

public class PaginationInfiniteResponseDto
{
    public IList<Fragrance> Fragrances { get; set; }
    public int CurrentPage { get; set; }
    public bool HasNextPage { get; set; }

    public PaginationInfiniteResponseDto(IList<Fragrance> fragrances, int currentPage, bool hasNextPage)
    {
        Fragrances = fragrances;
        CurrentPage = currentPage;
        HasNextPage = hasNextPage;
    }
}