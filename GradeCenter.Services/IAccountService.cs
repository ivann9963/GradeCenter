using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services
{
    public interface IAccountService
    {
        void Deactivate(User loggedUser);
        User GetUserById(string userId);
        string Login(string email, string password);
        Task Register(string userName, string email, string password);
        void UpdateUser(User loggedUser, string newPassword, string newPhoneNumber);
    }
}
