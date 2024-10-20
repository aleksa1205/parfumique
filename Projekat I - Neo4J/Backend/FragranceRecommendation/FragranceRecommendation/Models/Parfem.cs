namespace FragranceRecommendation.Models;

public class Parfem
{
    //nije dodato u bazu
    public string Slika { get; set; } = String.Empty;
    public string Naziv { get; set; }
    public int GodinaIzlaska { get; set; } = DateTime.Now.Year;
    public char Pol { get; set; }
    //lista gornjih nota
    //lista srednjih nota
    //lista donjih nota

    #region Constructors
    public Parfem(string naziv, int godina, char pol)
    {
        Naziv = naziv;
        GodinaIzlaska = godina;
        Pol = pol;
    }
    #endregion
}
