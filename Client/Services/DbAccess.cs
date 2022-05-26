using System.Text.Json;
using Api.Models;
using Blazored.LocalStorage;
using Client.Exceptions;
using Client.Models;
using Microsoft.AspNetCore.Components.Authorization;


namespace Client.Services;

public interface IDbAccess
{
    Task<string> SaveFavorite(FavoriteEntity favorite);
    Task<string> DeleteFavorite(string username,int movie_id);
    Task<string> GetFavorite(string username,string movie_id);
    
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
        var responseMsg = "failed";

        return responseMsg;
    }

    public async Task<string> GetFavorite(string username, string movie_id)
    {
        string responseBool="false";
        
        var response = await _httpClient.GetAsync($"http://localhost:7071/api/favorites/{username}/{movie_id}");
            
        
        if (response.IsSuccessStatusCode)
        {

            // var e = response.Content.ReadAsStringAsync();
            responseBool = "true";
        }

        return responseBool;
    }
}