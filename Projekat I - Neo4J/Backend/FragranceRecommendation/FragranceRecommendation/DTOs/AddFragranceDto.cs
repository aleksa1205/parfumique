namespace FragranceRecommendation.DTOs;

public class AddFragranceDto
{
    public required string Name { get; set; }
    public required char Gender { get; set; }
    public int BatchYear { get; set; }
}