namespace FragranceRecommendation.DTOs;

public class PagintaionResponseDto
{
    public int Skip { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IList<Fragrance> Fragrances { get; set; }

    public PagintaionResponseDto(int skip, int totalCount, int totalPages, IList<Fragrance> fragrances)
    {
        Skip = skip;
        TotalCount = totalCount;
        TotalPages = totalPages;
        Fragrances = fragrances;
    }
}