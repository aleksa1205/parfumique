namespace FragranceRecommendation.DTOs.NoteDTOs;

public class DeleteNoteDto
{
    public string Name { get; set; }
    
    public (bool isValid, string errorMessage) Validate()
    {
        var (isValid, errorMessage) = MyUtils.IsValidString(Name, "Name");
        return isValid ?(true, string.Empty) : ((false, errorMessage));
    }
}