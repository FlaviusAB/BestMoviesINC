using System.Net.Http.Headers;
using System.Net.Http.Json;
using Client.Models;
using Newtonsoft.Json;

namespace Client.Services;

public interface IDbAccess
{
    Task<bool> CheckNameByInput(string username);
    Task<string> RegisterUser(User user);
}

public class DbAccess : IDbAccess
{
    private HttpClient _httpClient = new HttpClient();

    public async Task<bool> CheckNameByInput(string username)
    {
        using var client = new HttpClient();
        var content = await client.GetStringAsync("http://localhost:7071/api/user/" + username);

        Console.WriteLine("HEREEEEEEEEEEEEEEEEE "+content);
        return true;
    }
    public async Task<string> RegisterUser(User user)
    {
        var responseMsg = "failed";
        using var client = new HttpClient();
        // var content = await client.GetStringAsync("http://localhost:7071/api/user/" + user.username);
        string content = "false";

        Console.WriteLine("COOOOOOO"+content);
        if (content.Equals("true"))
        {
            responseMsg = "username taken";
        }
        else
        {
            
            var userToSend = JsonConvert.SerializeObject(user);
            Console.WriteLine("1"+userToSend);
            
            var response = await _httpClient.PostAsJsonAsync("http://localhost:7071/api/signup", userToSend);
            
            Console.WriteLine("2"+response);
            if (response.IsSuccessStatusCode)
            {
                responseMsg = "user successfully registered";
            }
        }

        return responseMsg;
    }
}