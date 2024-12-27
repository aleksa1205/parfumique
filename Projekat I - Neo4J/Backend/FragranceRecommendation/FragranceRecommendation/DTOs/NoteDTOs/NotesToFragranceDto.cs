namespace FragranceRecommendation.DTOs.NoteDTOs;

public class NotesToFragranceDto
{
    [Range(0, int.MaxValue)]
    public int Id { get; set; }

    [Required]
    public IList<NoteDto>? Notes { get; set; }
}