using Newtonsoft.Json;

namespace Client.Models.Movies;

public class PeopleEntity
{
    [JsonProperty("adult")]
    public bool Adult { get; set; }
    [JsonProperty("biography")]
    public string? Biography { get; set; }
    [JsonProperty("birthday")]
    public string? Birthday { get; set; }
    [JsonProperty("deathday")]
    public object? deathday { get; set; }
    [JsonProperty("gender")]
    public int? Gender { get; set; }
    [JsonProperty("homepage")]
    public object? Homepage { get; set; }
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("imdb_id")]
    public string? ImdbId { get; set; }
    [JsonProperty("known_for_department")]
    public string? KnownForDepartment { get; set; }
    [JsonProperty("name")]
    public string? Name { get; set; }
    [JsonProperty("character")]
    public string? Character { get; set; }
    [JsonProperty("place_of_birth")]
    public string? PlaceOfBirth { get; set; }
    [JsonProperty("popularity")]
    public double Popularity { get; set; }
    [JsonProperty("profile_path")]
    public string? ProfilePath { get; set; }

    [JsonProperty("department")]
    public string? Department { get; set; }
}