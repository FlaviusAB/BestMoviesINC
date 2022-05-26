using System.ComponentModel.DataAnnotations;

namespace Client.Models.Account
{
    public class EditUser
    {
       
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }
        
        [MinLength(6, ErrorMessage = "The Password field must be a minimum of 6 characters")]
        public string Password { get; set; }

        public EditUser() { }

        public EditUser(User user)
        {
            
            Email = user.Email;
            Username = user.Username;
        }
    }
}