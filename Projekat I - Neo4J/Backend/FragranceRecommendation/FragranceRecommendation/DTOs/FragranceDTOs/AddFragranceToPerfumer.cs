namespace FragranceRecommendation.DTOs.FragranceDTOs;

public class AddFragranceToPerfumer
{
    [Range(0, int.MaxValue)]
    public int PerfumerId { get; set; }

    [Range(0, int.MaxValue)]
    public int FragranceId { get; set; }
}