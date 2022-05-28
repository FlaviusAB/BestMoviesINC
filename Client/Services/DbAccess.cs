
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
    Task<List<string>> GetAllFavorite(string username);
    

}

public class DbAccess : IDbAccess
{
    private readonly HttpClient _httpClient = new();

    public DbAccess()
    {
        _httpClient.BaseAddress = new Uri("https://bestmoviesfunction.azurewebsites.net/");
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
        Console.WriteLine("DbAccess DeleteFavorite: " + responseMsg);
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
        Console.WriteLine("DbAccess GetFavorite : "+responseBool);
        return responseBool;
    }

    public async Task<List<string>> GetAllFavorite(string username)
    {
        
        var response = await _httpClient.GetAsync($"api/favorites/{username}");
        response.EnsureSuccessStatusCode();
        
        string responseBody = await response.Content.ReadAsStringAsync();
        var favList = JsonConvert.DeserializeObject<List<string>>(responseBody).ToList();
        return favList;
    }
    
  
}