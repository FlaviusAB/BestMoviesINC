namespace Api.Models;

public class User
{
    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }
    public string SessionToken { get; set; }

    public string RegistrationDate { get; set; }
}