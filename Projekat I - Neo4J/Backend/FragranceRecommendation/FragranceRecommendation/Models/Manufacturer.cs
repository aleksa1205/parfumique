using Newtonsoft.Json;

namespace FragranceRecommendation.Models;

public class Manufacturer
{
    [JsonProperty("image")]
    public string Image { get; set; } = String.Empty;
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    public IList<Fragrance> Fragrances { get; set; } = new List<Fragrance>();

    #region Constructors

    public Manufacturer() { }
    public Manufacturer(string name)
    {
        Name = name;
    }
    #endregion
}
