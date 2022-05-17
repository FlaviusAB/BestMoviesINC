using Newtonsoft.Json;

namespace Client.Models;

public class TV
{
    
        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("created_by")]
        public List<CreatedBy> CreatedBy { get; set; }

        [JsonProperty("episode_run_time")]
        public List<int> EpisodeRunTime { get; set; }

        [JsonProperty("first_air_date")]
        public string FirstAirDate { get; set; }

        [JsonProperty("genres")]
        public List<Genre> Genres { get; set; }

        [JsonProperty("homepage")]
        public string Homepage { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("in_production")]
        public bool InProduction { get; set; }

        [JsonProperty("languages")]
        public List<string> Languages { get; set; }

        [JsonProperty("last_air_date")]
        public string LastAirDate { get; set; }

        [JsonProperty("last_episode_to_air")]
        public LastEpisodeToAir LastEpisodeToAir { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("next_episode_to_air")]
        public object NextEpisodeToAir { get; set; }

        [JsonProperty("networks")]
        public List<Network> Networks { get; set; }

        [JsonProperty("number_of_episodes")]
        public int NumberOfEpisodes { get; set; }

        [JsonProperty("number_of_seasons")]
        public int NumberOfSeasons { get; set; }

        [JsonProperty("origin_country")]
        public List<string> OriginCountry { get; set; }

        [JsonProperty("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonProperty("original_name")]
        public string OriginalName { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("production_companies")]
        public List<ProductionCompany> ProductionCompanies { get; set; }

        [JsonProperty("production_countries")]
        public List<ProductionCountry> ProductionCountries { get; set; }

        [JsonProperty("seasons")]
        public List<Season> Seasons { get; set; }

        [JsonProperty("spoken_languages")]
        public List<SpokenLanguage> SpokenLanguages { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tagline")]
        public string Tagline { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }
    
}