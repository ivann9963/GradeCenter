﻿using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services
{
    public interface IAccountService
    {
        void AddChild(AspNetUser parent, Guid childId);
        void Deactivate(AspNetUser loggedUser);
        AspNetUser GetUserById(string userId);
        Task<string> Login(string email, string password);
        Task Register(string userName, string email, string password);
        void UpdateUser(AspNetUser loggedUser, string newPassword, string newPhoneNumber);
    }
}
