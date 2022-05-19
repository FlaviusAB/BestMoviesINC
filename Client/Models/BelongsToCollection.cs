using Newtonsoft.Json;

namespace Client.Models;

public class BelongsToCollection
{
    [JsonProperty("id")]
    public int? Id;

    [JsonProperty("name")]
    public string? Name;

    [JsonProperty("poster_path")]
    public string? PosterPath;

    [JsonProperty("backdrop_path")]
    public string? BackdropPath;
}