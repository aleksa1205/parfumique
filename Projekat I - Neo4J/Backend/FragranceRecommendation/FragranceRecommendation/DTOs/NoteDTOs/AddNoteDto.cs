namespace FragranceRecommendation.DTOs.NoteDTOs;

public class AddNoteDto
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? Name { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? Type { get; set; }
}