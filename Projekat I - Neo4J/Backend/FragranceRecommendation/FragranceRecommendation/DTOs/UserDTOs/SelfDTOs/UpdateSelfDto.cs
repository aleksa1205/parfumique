using FragranceRecommendation.Common.ValidationAttributes;

namespace FragranceRecommendation.DTOs.UserDTOs.SelfDTOs;

public class UpdateSelfDto
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? Name { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? Surname { get; set; }

    [Required]
    [Gender]
    public char? Gender { get; set; }
}