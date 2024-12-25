using System.Runtime.InteropServices.JavaScript;
using Microsoft.OpenApi.MicrosoftExtensions;

namespace FragranceRecommendation.DTOs.NoteDTOs;

public class NotesToFragranceDto
{
    public int Id { get; set; }
    public IList<NoteDto> Notes { get; set; }

    public (bool isValid, string errorMessage) Validate()
    {
        if (Id < 0)
            return (false, "Fragrance Id cannot be less than zero!");
        
        foreach (var note in Notes)
        {
            var (isValid, errorMessage) = note.Validate();
            if(!isValid)
                return (false, errorMessage);
        }
        
        return (true, string.Empty);
    }
}