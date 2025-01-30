namespace FragranceRecommendation.DTOs.ManufacturerDTOs;

public class AddFragranceToManufacturerDto
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? ManufacturerName { get; set; }

    [Range(0, int.MaxValue)]
    public int FragranceId { get; set; }
}