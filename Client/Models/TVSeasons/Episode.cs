using Newtonsoft.Json;

namespace Client.Models.TVSeasons;

public class Episode
{
    [JsonProperty("air_date")]
    public string? AirDate { get; set; }

    [JsonProperty("episode_number")]
    public int EpisodeNumber { get; set; }

    [JsonProperty("crew")]
    public List<PeopleEntity>? PeopleEntities { get; set; }

    [JsonProperty("guest_stars")]
    public List<GuestStar>? GuestStars { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("overview")]
    public string? Overview { get; set; }

    [JsonProperty("production_code")]
    public string? ProductionCode { get; set; }

    [JsonProperty("runtime")]
    public int Runtime { get; set; }

    [JsonProperty("season_number")]
    public int SeasonNumber { get; set; }

    [JsonProperty("still_path")]
    public string? StillPath { get; set; }

    [JsonProperty("vote_average")]
    public double VoteAverage { get; set; }

    [JsonProperty("vote_count")]
    public int VoteCount { get; set; }
}