namespace FragranceRecommendation.Models;

public class Proizvodjac
{
    //nije dodato u bazu
    public string Slika { get; set; } = String.Empty;
    public string Naziv { get; set; }
    //lista parfema koje su proizveli

    #region Constructors
    public Proizvodjac(string naziv)
    {
        Naziv = naziv;
    }
    #endregion
}
