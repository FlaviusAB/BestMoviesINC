using System.Net.Http.Json;
using System.Text.Json;
using Api.Models;
using Client.Exceptions;
using Client.Models;



namespace Client.Services;

public interface IDbAccess
{
    Task<string> RegisterUser(User user);
    Task<string> UserAuthentication(UserCredentials user);
}

public class DbAccess : IDbAccess
{
    private HttpClient _httpClient = new HttpClient();
    
    public async Task<string> RegisterUser(User user)
    {
        var responseMsg = "failed";
        using var client = new HttpClient();
         
        string content = await client.GetStringAsync("http://localhost:7071/api/user/" + user.username);
        
        if (content.Equals("true"))
        {
            responseMsg = "username taken";
        }
        else
        {
            
            string message = JsonSerializer.Serialize(user);
            
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            var msg = new ByteArrayContent(messageBytes);
            var response = await _httpClient.PostAsync("http://localhost:7071/api/signup", msg);
            
            if (response.IsSuccessStatusCode)
            {
                
                responseMsg = "user successfully registered";
            }
        }

        return responseMsg;
    }

    public async Task<string> UserAuthentication(UserCredentials user)
    {
        var responseMsg = "failed";
        string message = JsonSerializer.Serialize(user);
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        var msg = new ByteArrayContent(messageBytes);
        var response = await _httpClient.PostAsync("http://localhost:7071/api/auth", msg);

        if (response.IsSuccessStatusCode)
        {
            responseMsg = "user successfully logged in";
        }
        else
        {
            responseMsg = "invalid information";
        }

        return responseMsg;
    }
}