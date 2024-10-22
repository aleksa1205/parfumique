namespace FragranceRecommendation.Models;

public class Nota
{
    public string Naziv { get; set; }
    public string Tip { get; set; }

    #region Constructors
    public Nota(string naziv, string tip)
    {
        Naziv = naziv;
        Tip = tip;
    }
    #endregion
}
