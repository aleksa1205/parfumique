namespace FragranceRecommendation.Models;

public class Korisnik
{
    //nije dodato u bazu
    public string Slika { get; set; } = String.Empty;
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public char Pol { get; set; } = 'U';
    public string KorisnickoIme { get; set; }
    public string Sifra { get; set; }
    //lista parfema koje poseduje
    
    #region Constructors
    public Korisnik(string ime, string prezime, char pol, string korisnickoIme, string sifra)
    {
        Ime = ime;
        Prezime = prezime;
        Pol = pol;
        KorisnickoIme = korisnickoIme;
        Sifra = sifra;
    }
    #endregion
}
