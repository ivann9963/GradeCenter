using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradeCenter.Data.Models.Account
{
    [Table("Users")]
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        [ForeignKey("School")]
        public string? SchoolId { get; set; }
        public virtual School School { get; set; }
        public bool IsActive { get; set; } = false;
        public UserRoles UserRole { get; set; }

        public ICollection<User> Students { get; set; } = new HashSet<User>();
        public ICollection<User> Parents { get; set; } = new HashSet<User>();
        public ICollection<User> Teachers { get; set; } = new HashSet<User>();
    }

    public enum UserRoles
    {
        Admin = 0,
        Principle = 1,
        Teacher = 2,
        Parent = 3,
        Student = 4
    }
}

