namespace FragranceRecommendation.DTOs;

public class DodajParfemDTO
{
    public required string Naziv { get; set; }
    public int GodinaIzlaska { get; set; } = 2024;
    public char Pol { get; set; } = 'U';
}