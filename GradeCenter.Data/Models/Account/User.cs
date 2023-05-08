using Microsoft.AspNetCore.Identity;

namespace GradeCenter.Data.Models.Account
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; } = false;
    }
}

