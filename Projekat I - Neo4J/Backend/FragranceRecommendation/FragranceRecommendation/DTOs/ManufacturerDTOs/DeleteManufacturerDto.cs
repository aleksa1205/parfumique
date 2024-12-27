namespace FragranceRecommendation.DTOs.ManufacturerDTOs;

public class DeleteManufacturerDto
{
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string? Name { get; set; }
}