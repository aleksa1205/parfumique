namespace FragranceRecommendation.DTOs.UserDTOs;

public class DeleteUserDto
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? Username { get; set; }
}