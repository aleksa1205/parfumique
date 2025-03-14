﻿namespace FragranceRecommendation.Models;

public class Note(string name, string type)
{
    [JsonProperty("image")]
    public string Image { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = name;
    
    [JsonProperty("type")]
    public string Type { get; set; } = type;
}
