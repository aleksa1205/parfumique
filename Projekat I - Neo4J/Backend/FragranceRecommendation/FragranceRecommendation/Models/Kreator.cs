namespace FragranceRecommendation.Models;

public class Kreator
{
    //nije dodato u bazu, funkcija setImage
    public string Slika { get; set; } = String.Empty;
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public string Drzava { get; set; }
    public int GodinaRodjenja { get; set; } = 0;
    //lista parfema koje je kreirao

    #region Constructors
    public Kreator(string ime, string prezime, string drzava)
    {
        Ime = ime;
        Prezime = prezime;
        Drzava = drzava;
    }

    public Kreator(string ime, string prezime, string drzava, int godinaRodjenja)
    {
        Ime = ime;
        Prezime = prezime;
        Drzava = drzava;
        GodinaRodjenja = godinaRodjenja;
    }
    #endregion
}
