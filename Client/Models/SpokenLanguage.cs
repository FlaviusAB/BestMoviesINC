using Newtonsoft.Json;

namespace Client.Models
{
    public class SpokenLanguage
    {
        [JsonProperty("english_name")]
        public string EnglishName;

        [JsonProperty("iso_639_1")]
        public string Iso6391;

        [JsonProperty("name")]
        public string Name;
    }
}