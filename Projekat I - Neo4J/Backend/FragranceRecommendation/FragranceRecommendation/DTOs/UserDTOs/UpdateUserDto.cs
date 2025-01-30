using FragranceRecommendation.Common.ValidationAttributes;

namespace FragranceRecommendation.DTOs.UserDTOs;

public class UpdateUserDto
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? Username { get; set; }

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