using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradeCenter.Data.Models.Account
{
    [Table("AspNetUsers")]
    public class AspNetUser : IdentityUser<Guid>
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }

        [ForeignKey("School")]
        public string? SchoolId { get; set; }
        public virtual School? School { get; set; }
        public bool? IsActive { get; set; } = false;
        public UserRoles? UserRole { get; set; }
        public SchoolClass? SchoolClass { get; set; }

        public virtual ICollection<UserRelation> ChildrenRelations { get; set; } = new HashSet<UserRelation>();
        public virtual ICollection<UserRelation> ParentRelations { get; set; } = new HashSet<UserRelation>();
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

