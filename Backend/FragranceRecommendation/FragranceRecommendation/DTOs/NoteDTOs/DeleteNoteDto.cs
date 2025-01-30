namespace FragranceRecommendation.DTOs.NoteDTOs;

public class DeleteNoteDto
{
    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string? Name { get; set; }
}