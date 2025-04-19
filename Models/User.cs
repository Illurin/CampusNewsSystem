using System.ComponentModel.DataAnnotations;

namespace CampusNewsSystem.Models
{
    public class User
    {
        public enum Auth
        {
            Admin, User
        }

        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public Auth Role { get; set; } = Auth.User;

        public bool IsEmailConfirmed { get; set; } = false;
    }
}
