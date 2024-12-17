namespace FragranceRecommendation.DTOs;

public class UpdatePerfumerDto
{
    public required int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public char Gender { get; set; }
    public string Country { get; set; }
}