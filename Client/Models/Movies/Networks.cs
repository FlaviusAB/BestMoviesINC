using Newtonsoft.Json;

namespace Client.Models.Movies;

public class Networks
{
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("logo_path")]
    public string? LogoPath { get; set; }
    
    [JsonProperty("origin_country")]
    public string? OriginCountry { get; set; }
}