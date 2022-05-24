namespace Client.Models;

public class User
{
    
    public string username { get; set; }
    public string password { get; set; }
    public string email { get; set; }
    public string accessType { get; set; }
    
   

    public User(string username, string password, string email, string accessType)
    {
        this.username = username;
        this.password = password;
        this.email = email;
        this.accessType = accessType;
        this.email = email;
    }
}