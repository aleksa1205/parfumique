namespace FragranceRecommendation.DTOs.NoteDTOs;

public class NoteDto
{
    public required string Name { get; set; }
    public int TMB { get; set; }

    public (bool isValid, string errorMessage) Validate()
    {
        var (isValid, errorMessage) = MyUtils.IsValidString(Name, "Name");
        if (!isValid)
            return (false, errorMessage);

        if (TMB < 0 || TMB > 2)
            return (false, "TMB must be between 0 and 2!");
        
        return (true, string.Empty);
    }
}