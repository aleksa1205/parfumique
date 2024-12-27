namespace FragranceRecommendation.DTOs.NoteDTOs;

public class NoteDto
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? Name { get; set; }

    [Required]
    [Range(0, 2)]
    public int? TMB { get; set; }
}