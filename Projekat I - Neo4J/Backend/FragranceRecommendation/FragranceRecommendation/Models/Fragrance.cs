using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace FragranceRecommendation.Models;

public class Fragrance
{
    [JsonProperty("id")]
    public int? Id { get; set; } = null;
    
    [JsonPropertyName("image")]
    public string Image { get; set; } = String.Empty;
    
    [JsonProperty("name")]
    public string Name { get; set; }
    //tip eau de toilet, eau de parfum, parfum ,extract
    
    [JsonProperty("for")]
    public char Gender { get; set; }
    
    [JsonProperty("year")]
    public int BatchYear { get; set; } = DateTime.Now.Year;
    
    public Manufacturer? Manufacturer { get; set; } = null;
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
