using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models
{
    public class User
    {
        public const string Admin = "Admin";
        public const string UserRole = "User";

        public virtual int Id { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public virtual string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public virtual string Email { get; set; }

        public virtual string PasswordHash { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public virtual string Role { get; set; }

        public virtual DateTime CreatedAt { get; set; }

        public User()
        {
            Role = Admin;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
