namespace FragranceRecommendation.DTOs.ManufacturerDTOs;

public class AddManufacturerDto
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? Name { get; set; }
}