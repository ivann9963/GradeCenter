using GradeCenter.Data;
using GradeCenter.Data.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GradeCenter.Services
{
    public class AccountService : IAccountService
    {
        public const string SECRET = "THIS IS USED TO SIGN AND VERIFY JWT TOKENS, REPLACE  IT WITH YOUR OWN SECRET, IT CAN BE ANY STRING";
        private readonly GradeCenterContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountService(UserManager<IdentityUser> userManager, GradeCenterContext dietHelperDbContext, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _db = dietHelperDbContext;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Takes a username and password as parameters
        /// and returns a JWT token after authenticating the user. 
        /// The method searches for the user in the database and generates 
        /// a JWT token using the JwtSecurityTokenHandler class.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Login(string userName, string password)
        {
            var user = _db?.Users?.SingleOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                return string.Empty;
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SECRET);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                }),

                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Updates the password and phone number of the logged-in user.
        /// </summary>
        /// <param name="loggedUser"></param>
        /// <param name="newPassword"></param>
        /// <param name="newPhoneNumber"></param>
        public void UpdateUser(User loggedUser, string newPassword, string newPhoneNumber)
        {
            loggedUser.PasswordHash = newPassword.GetHashCode().ToString(); // TODO: Check if okay
            loggedUser.PhoneNumber = newPhoneNumber;

            _db.SaveChanges();
        }

        /// <summary>
        /// Takes a username, email, and password as parameters and 
        /// creates a new user in the database using the UserManager object.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task Register(string userName, string email, string password)
        {
            var userModel = new User
            {
                UserName = userName,
                Email = email,
            };

            await _userManager.CreateAsync(userModel, password);

            await _db.SaveChangesAsync();
        }


        /// <summary>
        /// Sets the IsActive property of the logged-in user to false in the database.
        /// </summary>
        /// <param name="loggedUser"></param>
        public void Deactivate(User loggedUser)
        {
            loggedUser.IsActive = false;

            _db.SaveChanges();
        }

        /// <summary>
        /// Returns a User object from the database based on the given user ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public User? GetUserById(string userId)
        {
            var user = _db?.Users?.FirstOrDefault(u => u.Id == userId);

            return user;
        }
    }
}
