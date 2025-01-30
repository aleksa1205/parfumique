namespace FragranceRecommendation.DTOs.ManufacturerDTOs;

public class UpdateManufacturerDto
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? Name { get; set; }

    [Required]
    public string? Image { get; set; }
}