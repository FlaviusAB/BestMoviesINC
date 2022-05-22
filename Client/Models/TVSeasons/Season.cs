using Newtonsoft.Json;

namespace Client.Models.TVSeasons;

public class Season
{
    [JsonProperty("_id")]
    public string _Id { get; set; }

    [JsonProperty("air_date")]
    public string AirDate { get; set; }

    [JsonProperty("episodes")]
    public List<Episode> Episodes { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("overview")]
    public string Overview { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("poster_path")]
    public string PosterPath { get; set; }

    [JsonProperty("season_number")]
    public int SeasonNumber { get; set; }
    
}