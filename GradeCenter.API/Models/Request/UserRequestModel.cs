using GradeCenter.Data.Models.Account;

namespace GradeCenter.API.Models.Request
{
    public class UserRequestModel
    {
        public Guid? UserId { get; set; }

        public UserRoles? Role { get; set; }
    }
}
