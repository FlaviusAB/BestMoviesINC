using Newtonsoft.Json;

namespace Client.Models;

public class CreatedBy
{
    
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("credit_id")]
    public string? CreditId { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("gender")]
    public int Gender { get; set; }

    [JsonProperty("profile_path")]
    public string? ProfilePath { get; set; }
}