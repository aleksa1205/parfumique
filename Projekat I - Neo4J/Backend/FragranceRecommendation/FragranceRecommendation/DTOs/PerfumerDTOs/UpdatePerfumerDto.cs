using FragranceRecommendation.Common.ValidationAttributes;

namespace FragranceRecommendation.DTOs.PerfumerDTOs;

public class UpdatePerfumerDto
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [StringLength(30, MinimumLength = 3)]
    public string? Name { get; set; }

    [StringLength(30, MinimumLength = 3)]
    public string? Surname { get; set; }

    [Gender]
    public char? Gender { get; set; }

    [StringLength(30, MinimumLength = 3)]
    public string? Country { get; set; }

    public string? Image { get; set; }
}