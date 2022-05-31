using Newtonsoft.Json;

namespace Client.Models.Movies
{
    public class ProductionCountry
    {
        [JsonProperty("iso_3166_1")]
        public string? Iso31661;

        [JsonProperty("name")]
        public string? Name;
    }
}