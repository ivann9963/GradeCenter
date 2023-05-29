using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services
{
    public interface IAccountService
    {
        void AddChild(Guid parentId, string childFirstName, string childLastName);
        void Deactivate(AspNetUser loggedUser);
        AspNetUser GetUserById(string userId);
        AspNetUser GetUserByNames(string firstLastName);
        Task<string> Login(string email, string password);
        Task Register(string userName, string email, string password);
        void UpdateUser(string? userId, string? newPassword, UserRoles? newRole, bool? isActive, string? newPhoneNumber);
        IEnumerable<AspNetUser> GetAllUsers();
    }
}
