namespace FragranceRecommendation.DTOs.FragranceDTOs;

public class DeleteFragranceDto
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }
}