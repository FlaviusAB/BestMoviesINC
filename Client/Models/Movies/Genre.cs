using Newtonsoft.Json;

namespace Client.Models.Movies
{
    public class Genre
    {
        [JsonProperty("id")] 
        public int? Id;

        [JsonProperty("name")] 
        public string? Name;
    }
}