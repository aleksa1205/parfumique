namespace FragranceRecommendation.DTOs;

public class PaginationResponseDto
{
    public int Skip { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IList<Fragrance> Fragrances { get; set; }

    public PaginationResponseDto(int skip, int totalCount, int totalPages, IList<Fragrance> fragrances)
    {
        Skip = skip;
        TotalCount = totalCount;
        TotalPages = totalPages;
        Fragrances = fragrances;
    }
}