namespace Client.Models
{
    public class User
    {
        public User(string username, string email, string sessionToken, string registrationDate)
        {
            Username = username;
            Email = email;
            SessionToken = sessionToken;
            RegistrationDate = registrationDate;
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public string SessionToken { get; set; }
        public string RegistrationDate { get; set; }
        
    }
}