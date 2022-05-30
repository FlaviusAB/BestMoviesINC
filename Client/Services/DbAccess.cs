
using Api.Models;
using Client.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Client.Services;

public interface IDbAccess
{
    Task<string> SaveFavorite(FavoriteEntity favorite);
    Task<string> DeleteFavorite(string username,int movie_id);
    Task<string> GetFavorite(string username,int movie_id);
    Task<List<string>?> GetAllFavorite(string username);
    Task<string> GetFavoriteCount(int movie_id);
    Task<List<string>?> GetAllUsersFavorite();
    Task<string> SaveReview(MovieReviewEntity review);
    Task<List<MovieReviewEntity>> GetReviews(int movie_id);

}

public class DbAccess : IDbAccess
{
    private readonly HttpClient _httpClient = new();

    public DbAccess()
    {
        _httpClient.BaseAddress = new Uri("http://localhost:7071/");
    }

    public async Task<string> SaveFavorite(FavoriteEntity favorite)
    {
        var responseMsg = "failed";

        string message = JsonSerializer.Serialize(favorite);
            
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        var msg = new ByteArrayContent(messageBytes);
        var response = await _httpClient.PostAsync("api/favorites", msg);
            
        
        if (response.IsSuccessStatusCode)
        {
                
            responseMsg = "Movie successfully added to the favorites!";
        }

        return responseMsg;
    }

    public async Task<string> DeleteFavorite(string username,int movie_id)
    {
        var responseMsg = "Failed";

        var response = await _httpClient.DeleteAsync($"api/favorites/{username}/{movie_id}");
        
        if (response.IsSuccessStatusCode)
        {
            responseMsg = "Success!";
        }
        return responseMsg;
    }

    public async Task<string> GetFavorite(string username, int movie_id)
    {
        string responseBool="false";
        
        var response = await _httpClient.GetStringAsync($"api/favorites/{username}/{movie_id}");
        
        if (response.Equals("true"))
        {
            responseBool = "true";
        }
        return responseBool;
    }

    public async Task<List<string>?> GetAllFavorite(string username)
    {
        
        var response = await _httpClient.GetAsync($"api/favorites/{username}");
        response.EnsureSuccessStatusCode();
        
        string responseBody = await response.Content.ReadAsStringAsync();
        var favList = JsonConvert.DeserializeObject<List<string>>(responseBody).ToList();
        return favList;
    }

    public async Task<string> GetFavoriteCount(int movie_id)
    {
        var response = await _httpClient.GetStringAsync($"api/favoriteCount/{movie_id}");
        return response;
    }

    public async Task<List<string>?> GetAllUsersFavorite()
    {
        var response = await _httpClient.GetAsync($"api/allFavorite/All");
        response.EnsureSuccessStatusCode();
        
        string responseBody = await response.Content.ReadAsStringAsync();
        var favList = JsonConvert.DeserializeObject<List<string>>(responseBody).ToList();
        return favList;
    }

    public async Task<string> SaveReview(MovieReviewEntity review)
    {
        var responseMsg = "failed";

        string message = JsonSerializer.Serialize(review);
            
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        var msg = new ByteArrayContent(messageBytes);
        var response = await _httpClient.PostAsync("api/review", msg);
            
        
        if (response.IsSuccessStatusCode)
        {
                
            responseMsg = "Review successfully added to the movie!";
        }

        return responseMsg;
    }
    
    public async Task<List<MovieReviewEntity>> GetReviews(int movie_id)
    {
        var revList = new List<MovieReviewEntity>();
        var response = await _httpClient.GetAsync($"api/review/{movie_id}");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        
        revList = JsonConvert.DeserializeObject<List<MovieReviewEntity>>(responseBody);
         for (int i = 0; i < revList.Count; i++)
         {
             Console.WriteLine("DBACCESS: " + revList[i].review);
         }
        
        return revList;
    }
    
}