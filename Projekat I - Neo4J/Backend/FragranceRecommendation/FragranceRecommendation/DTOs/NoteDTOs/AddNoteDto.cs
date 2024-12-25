namespace FragranceRecommendation.DTOs.NoteDTOs;

public class AddNoteDto
{
    public string Name { get; set; }
    public string Type { get; set; }
    
    public (bool isValid, string errorMessage) Validate()
    {
        var (isValid, errorMessage) = MyUtils.IsValidString(Name, "Name");
        if(!isValid)
            return (false, errorMessage);
        
        (isValid, errorMessage) = MyUtils.IsValidString(Type, "Type");
        if(!isValid)
            return (false, errorMessage);

        return (true, string.Empty);
    }
}