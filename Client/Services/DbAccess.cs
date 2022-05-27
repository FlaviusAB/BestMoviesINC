using Api.Models;
using Blazored.LocalStorage;
using Client.Exceptions;
using Client.Models;
using Microsoft.AspNetCore.Components.Authorization;
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
    private HttpClient _httpClient = new HttpClient();
    // private readonly ILocalStorageService _localStorage ;
    // private readonly AuthenticationStateProvider _authStateProvider;

    public async Task<string> SaveFavorite(FavoriteEntity favorite)
    {
        var responseMsg = "failed";

        string message = JsonSerializer.Serialize(favorite);
            
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        var msg = new ByteArrayContent(messageBytes);
        var response = await _httpClient.PostAsync("http://localhost:7071/api/favorites", msg);
            
        
        if (response.IsSuccessStatusCode)
        {
                
            responseMsg = "Movie successfully added to the favorites!";
        }

        return responseMsg;
    }

    public async Task<string> DeleteFavorite(string username,int movie_id)
    {
        var responseMsg = "Failed";

        var response = await _httpClient.DeleteAsync($"http://localhost:7071/api/favorites/{username}/{movie_id}");
        
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
        
        var response = await _httpClient.GetStringAsync($"http://localhost:7071/api/favorites/{username}/{movie_id}");
        
        if (response.Equals("true"))
        {
            responseBool = "true";
        }
        Console.WriteLine("DbAccess GetFavorite : "+responseBool);
        return responseBool;
    }

    public async Task<List<string>> GetAllFavorite(string username)
    {
        Console.WriteLine("GET ALL FAVORITES dbaccess");
        List<string> AllMovies = new List<string>();
        var response = await _httpClient.GetAsync($"http://localhost:7071/api/favorites/{username}");
        response.EnsureSuccessStatusCode();
        
        Console.WriteLine("break1");
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("break1.5");
        string obj = JsonConvert.DeserializeObject<string>(responseBody);
        Console.WriteLine("obj: " + obj);
        AllMovies.Add(obj);
        // Console.WriteLine("break1.5");
        // var resultObjects = AllChildren(JObject.Parse(responseBody))
        //     .First(c => c.Type == JTokenType.Array && c.Path.Contains(""))
        //     .Children<JObject>();
        //
        //
        // Console.WriteLine("break2");
        // foreach (JObject result in resultObjects)
        // {
        //     var obj = JsonConvert.DeserializeObject<string>(result.ToString());
        //     Console.WriteLine("foreach" + obj);
        //     AllMovies.Add(obj);
        // }
        // Console.WriteLine("break3");
        // for (int i = 0; i < AllMovies.Count; i++)
        // {
        //     Console.WriteLine("FOR LOOP: "+AllMovies[i]);
        // }
        // return AllMovies;
        return null;
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
}