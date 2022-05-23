namespace Api.Models;

public class User
{
    
    public string username { get; set; }
    public string password { get; set; }
    public string email { get; set; }
    public string accessType { get; set; }
    


    public User(string username, string password, string accessType, string email)
    {
        this.username = username;
        this.password = password;
        this.accessType = accessType;
        this.email = email;
    }
}