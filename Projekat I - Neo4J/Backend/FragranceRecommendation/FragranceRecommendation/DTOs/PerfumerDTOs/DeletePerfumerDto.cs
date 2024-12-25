namespace FragranceRecommendation.DTOs.PerfumerDTOs;

public class DeletePerfumerDto
{
    public int Id { get; set; }
    
    public (bool isValid, string errorMessage) Validate()
    {
        return Id > 0 ? (true, string.Empty) : (false, "Perfumer ID must be a positive integer!");
    }
}