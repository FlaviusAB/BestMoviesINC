using Client.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Client.Services;

public interface IMoviesData
{
    Task<List<Movie>> GetTrending();
    Task<Movie> GetMovieDetails(string id);
    Task<List<PeopleEntity>> GetMovieCast(string id);
    Task<List<Movie>> GetSimilar(string id);
    Task<PeopleEntity> GetMovieCastSingle(string id);
}

public class MoviesData : IMoviesData
{
    private readonly HttpClient client = new();
    

    public async Task<List<Movie>> GetTrending()
    {
        var movies = new List<Movie>();
        var response =
            await client.GetAsync(
                "https://api.themoviedb.org/3/trending/movie/week?api_key=a5ab4805002668ee4999f8bac7a4691d");
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        var resultObjects = AllChildren(JObject.Parse(responseBody))
            .First(c => c.Type == JTokenType.Array && c.Path.Contains("results"))
            .Children<JObject>();

        foreach (var result in resultObjects)
        {
            var obj = JsonConvert.DeserializeObject<Movie>(result.ToString());
            movies.Add(obj);
        }

        return movies;
    }

    public async Task<Movie> GetMovieDetails(string id)
    {
        HttpResponseMessage response = await client.GetAsync("https://api.themoviedb.org/3/movie/"+id+"?api_key=a5ab4805002668ee4999f8bac7a4691d&language=en-US");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var obj = JsonConvert.DeserializeObject<Movie>(responseBody);
        return obj;
    }

    public async Task<List<PeopleEntity>> GetMovieCast(string id)
    {
        var cast = new List<PeopleEntity>();
        HttpResponseMessage response = await client.GetAsync("https://api.themoviedb.org/3/movie/" + id + "/credits?api_key=a5ab4805002668ee4999f8bac7a4691d&language=en-US");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var resultObjects = AllChildren(JObject.Parse(responseBody))
            .First(c => c.Type == JTokenType.Array && c.Path.Contains("cast"))
            .Children<JObject>();
        
        foreach (JObject result in resultObjects)
        {
            var obj = JsonConvert.DeserializeObject<PeopleEntity>(result.ToString());
            cast.Add(obj);
        }

        return cast;
    }
    
    public async Task<PeopleEntity> GetMovieCastSingle(string id)
    {
        HttpResponseMessage response = await client.GetAsync("https://api.themoviedb.org/3/person/" + id + "?api_key=a5ab4805002668ee4999f8bac7a4691d&language=en-US");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var obj = JsonConvert.DeserializeObject<PeopleEntity>(responseBody);
        return obj;
    }

    private static IEnumerable<JToken> AllChildren(JToken json)
    {
        foreach (var c in json.Children())
        {
            yield return c;
            foreach (var cc in AllChildren(c))
            {
                yield return cc;
            }
        }
    }

    public async Task<List<Movie>> GetSimilar(string id)
    {
        var movies = new List<Movie>();
        
        var response =
            await client.GetAsync(
                "https://api.themoviedb.org/3/movie/similar/" + id +"/week?api_key=a5ab4805002668ee4999f8bac7a4691d");
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        var resultObjects = AllChildren(JObject.Parse(responseBody))
            .First(c => c.Type == JTokenType.Array && c.Path.Contains("results"))
            .Children<JObject>();

        foreach (var result in resultObjects)
        {
            var obj = JsonConvert.DeserializeObject<Movie>(result.ToString());
            movies.Add(obj);
        }
        return movies;
    }
}
