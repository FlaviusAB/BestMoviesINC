using Newtonsoft.Json;

namespace Client.Models;

public class PeopleEntity
{
    [JsonProperty("adult")]
    public bool adult { get; set; }
    [JsonProperty("biography")]
    public string biography { get; set; }
    [JsonProperty("birthday")]
    public string birthday { get; set; }
    [JsonProperty("deathday")]
    public object deathday { get; set; }
    [JsonProperty("gender")]
    public int gender { get; set; }
    [JsonProperty("homepage")]
    public object homepage { get; set; }
    [JsonProperty("id")]
    public int id { get; set; }
    [JsonProperty("imdb_id")]
    public string imdb_id { get; set; }
    [JsonProperty("known_for_department")]
    public string known_for_department { get; set; }
    [JsonProperty("name")]
    public string name { get; set; }
    [JsonProperty("character")]
    public string character { get; set; }
    [JsonProperty("place_of_birth")]
    public string place_of_birth { get; set; }
    [JsonProperty("popularity")]
    public double popularity { get; set; }
    [JsonProperty("profile_path")]
    public string profile_path { get; set; }

    [JsonProperty("department")]
    public string department { get; set; }
}