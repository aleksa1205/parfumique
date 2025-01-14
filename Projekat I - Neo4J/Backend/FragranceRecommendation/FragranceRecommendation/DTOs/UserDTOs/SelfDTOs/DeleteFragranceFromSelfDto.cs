namespace FragranceRecommendation.DTOs.UserDTOs.SelfDTOs;

public class DeleteFragranceFromSelfDto
{
    [Range(0, int.MaxValue)] public int FragranceId { get; set; }
}