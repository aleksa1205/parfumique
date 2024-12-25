namespace FragranceRecommendation.DTOs.FragranceDTOs;

public class AddFragranceToPerfumer
{
    public int PerfumerId { get; set; }
    public int FragranceId { get; set; }
    
    
    public (bool isValid, string errorMessage) Validate()
    {
        if (PerfumerId < 0)
            return (false, "Perfumer ID must be a positive integer!");
        
        if (FragranceId < 0)
            return (false, "Fragrance ID must be a positive integer!");

        return (true, string.Empty);
    }
}