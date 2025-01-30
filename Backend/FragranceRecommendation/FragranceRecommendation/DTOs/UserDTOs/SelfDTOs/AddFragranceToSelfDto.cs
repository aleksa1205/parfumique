namespace FragranceRecommendation.DTOs.UserDTOs.SelfDTOs;

public class AddFragranceToSelfDto
{
    [Range(0, int.MaxValue)]
    public int FragranceId { get; set; }
}