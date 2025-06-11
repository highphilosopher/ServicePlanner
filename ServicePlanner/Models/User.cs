using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ServicePlanner.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; } = UserRole.User;

        public bool IsFirstLogin { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Computed property for display
        public string FullName => $"{FirstName} {LastName}";
    }
}
