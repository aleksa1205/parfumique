namespace FragranceRecommendation.DTOs;

public class DodajKreatoraDTO
{
    public required string Ime { get; set; }
    public required string Prezime { get; set; }
    public required string Drzava { get; set; }
    public int GodinaRodjenja { get; set; } = 0;
}
