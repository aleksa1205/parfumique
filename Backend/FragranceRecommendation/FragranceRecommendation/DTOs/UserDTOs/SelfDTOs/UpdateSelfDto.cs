using FragranceRecommendation.Common.ValidationAttributes;

namespace FragranceRecommendation.DTOs.UserDTOs.SelfDTOs;

public class UpdateSelfDto
{
    [StringLength(30, MinimumLength = 3)]
    public string? Name { get; set; }

    [StringLength(30, MinimumLength = 3)]
    public string? Surname { get; set; }

    [Gender]
    public char? Gender { get; set; }

    public string? Image { get; set; }
}