using Newtonsoft.Json;

namespace Client.Models
{
    public class Movie
    {
        [JsonProperty("adult")] public bool Adult;

        [JsonProperty("backdrop_Path")] public string? BackdropPath;

        [JsonProperty("belongs_to_collection")]
        public BelongsToCollection? MovieCollection;

        [JsonProperty("budget")] public Int64? Budget;

        [JsonProperty("genres")] public List<Genre>? Genres;

        [JsonProperty("homepage")] public string? Homepage;

        [JsonProperty("id")] public int Id;

        [JsonProperty("imdb_id")] public string? ImdbId;

        [JsonProperty("original_language")] public string? OriginalLanguage;

        [JsonProperty("original_title")] public string? OriginalTitle;

        [JsonProperty("overview")] public string? Overview;

        [JsonProperty("poster_path")] public string? PosterPath;

        [JsonProperty("popularity")] public double Popularity;

        [JsonProperty("production_companies")] public List<ProductionCompany>? ProductionCompanies;

        [JsonProperty("production_countries")] public List<ProductionCountry>? ProductionCountries;

        [JsonProperty("release_date")] public string? ReleaseDate;

        [JsonProperty("revenue")] public Int64? Revenue;

        [JsonProperty("runtime")] public int? Runtime;

        [JsonProperty("spoken_languages")] public List<SpokenLanguage>? SpokenLanguages;

        [JsonProperty("status")] public string? Status;

        [JsonProperty("tagline")] public string? Tagline;

        [JsonProperty("title")] public string? Title;

        [JsonProperty("video")] public bool Video;

        [JsonProperty("vote_average")] public double VoteAverage;

        [JsonProperty("vote_count")] public int? VoteCount;
    }
}