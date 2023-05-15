using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services
{
    public interface IAccountService
    {
        void AddChild(User parent, Guid childId);
        void Deactivate(User loggedUser);
        User GetUserById(string userId);
        Task<string> Login(string email, string password);
        Task Register(string userName, string email, string password);
        void UpdateUser(User loggedUser, string newPassword, string newPhoneNumber);
    }
}
