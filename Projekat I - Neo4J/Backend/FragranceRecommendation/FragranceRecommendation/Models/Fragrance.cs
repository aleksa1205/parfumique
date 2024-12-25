namespace FragranceRecommendation.Models;

public class Fragrance
{
    [JsonProperty("id")]
    public int? Id { get; set; } = null;
    
    [JsonProperty("image")]
    public string Image { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    //tip eau de toilet, eau de parfum, parfum ,extract
    
    [JsonProperty("gender")]
    public char Gender { get; set; }
    
    [JsonProperty("year")]
    public int BatchYear { get; set; }
    
    public Manufacturer? Manufacturer { get; set; } = null;
    public IList<Perfumer> Perfumers { get; set; } = new List<Perfumer>();
    public IList<Note> Top { get; set; } = new List<Note>();
    public IList<Note> Middle { get; set; } = new List<Note>();
    public IList<Note> Base { get; set; } = new List<Note>();
    
    #region Constructors
    public Fragrance() {}

    public Fragrance(string name)
    {
        Name = name;
    }

    public Fragrance(string name, char gender, int year)
    {
        Name = name;
        Gender = gender;
        BatchYear = year;
    }
    #endregion
}
