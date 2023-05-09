using Microsoft.AspNetCore.Identity;

namespace GradeCenter.Data.Models.Account
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; } = false;
        public UserRoles UserRole { get; set; }
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

