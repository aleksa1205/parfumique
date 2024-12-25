namespace FragranceRecommendation.DTOs.FragranceDTOs;

public class DeleteFragranceDto
{
    public int Id { get; set; }
    
    public (bool isValid, string errorMessage) Validate()
    {
        return Id > 0 ? (true, string.Empty) : (false, "Fragrance ID must be a positive integer!");
    }
}