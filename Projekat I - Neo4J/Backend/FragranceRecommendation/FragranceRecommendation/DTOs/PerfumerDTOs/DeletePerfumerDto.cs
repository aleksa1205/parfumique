namespace FragranceRecommendation.DTOs.PerfumerDTOs;

public class DeletePerfumerDto
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }
}