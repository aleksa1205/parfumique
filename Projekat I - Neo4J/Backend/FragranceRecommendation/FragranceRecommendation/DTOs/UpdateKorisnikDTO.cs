namespace FragranceRecommendation.DTOs;

public class UpdateKorisnikDTO
{    
    public string KorisnickoIme { get; set; }
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public char Pol { get; set; } = 'M';
}