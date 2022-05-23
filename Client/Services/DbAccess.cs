using System.Net.Http.Json;
using System.Text.Json;
using Client.Models;



namespace Client.Services;

public interface IDbAccess
{
  
    Task<string> RegisterUser(User user);
}

public class DbAccess : IDbAccess
{
    private HttpClient _httpClient = new HttpClient();
    
    public async Task<string> RegisterUser(User user)
    {
        var responseMsg = "failed";
        using var client = new HttpClient();
         
        string content = await client.GetStringAsync("https://bestmoviesfunction.azurewebsites.net/api/user/" + user.username);
        
        if (content.Equals("true"))
        {
            responseMsg = "username taken";
        }
        else
        {
            
            string message = JsonSerializer.Serialize(user);
            
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            var msg = new ByteArrayContent(messageBytes);
            var response = await _httpClient.PostAsync("https://bestmoviesfunction.azurewebsites.net/api/signup", msg);
            
            if (response.IsSuccessStatusCode)
            {
                responseMsg = "user successfully registered";
            }
        }

        return responseMsg;
    }
}