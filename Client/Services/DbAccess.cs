using System.Net.Http.Headers;

namespace Client.Services;

public interface IDbAccess
{
    Task<bool> CheckNameByInput(string username);
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
}