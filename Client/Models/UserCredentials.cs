namespace Api.Models;

public class UserCredentials {
    public UserCredentials(string username, string password)
    {
        this.username = username;
        this.password = password;
    }

    public string username {
        get;
        set;
    }
    public string password {
        get;
        set;
    }

    public string authToken
    {
        get;
        set;
    }
}