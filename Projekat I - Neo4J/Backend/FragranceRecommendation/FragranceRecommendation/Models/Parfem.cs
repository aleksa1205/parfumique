namespace FragranceRecommendation.Models;

public class Parfem
{
    //nije dodato u bazu
    public string Slika { get; set; } = String.Empty;
    public string Naziv { get; set; }
    public int GodinaIzlaska { get; set; } = DateTime.Now.Year;
    public char Pol { get; set; } = 'U';
    public Proizvodjac Proizvodjac { get; set; } = null;
    public IList<Nota> Gornje { get; set; } = new List<Nota>();
    public IList<Nota> Srednje { get; set; } = new List<Nota>();
    public IList<Nota> Donje { get; set; } = new List<Nota>();


    #region Constructors
    public Parfem(string naziv, int godina, char pol)
    {
        Naziv = naziv;
        GodinaIzlaska = godina;
        Pol = pol;
    }
    #endregion
}
