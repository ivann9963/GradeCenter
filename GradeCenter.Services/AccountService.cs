using GradeCenter.Data;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly UserManager<AspNetUser> _userManager;
        private readonly SignInManager<AspNetUser> _signInManager;

        public AccountService(UserManager<AspNetUser> userManager, GradeCenterContext dietHelperDbContext, SignInManager<AspNetUser> signInManager)
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
        public async Task<string> Login(string userName, string password)
        {
            var user = _db?.AspNetUsers?.SingleOrDefault(u => u.UserName == userName);

            if (user == null)
            {
                return string.Empty;
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
                return string.Empty;

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
        public void UpdateUser(string? userId, string? newPassword, UserRoles? newRole, bool? isActive, string? newPhoneNumber)
        {
            var user = _db?.AspNetUsers?.FirstOrDefault(u => u.Id == Guid.Parse(userId));

            if(newPassword != null)
                user.PasswordHash = newPassword.GetHashCode().ToString();

            if(newRole != null)
                user.UserRole = newRole;

            if(newPhoneNumber != null)
                user.PhoneNumber = newPhoneNumber;


            if (isActive != null)
                user.IsActive = isActive;

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
            var userModel = new AspNetUser
            {
                FirstName = userName,
                LastName = userName,
                UserName = userName,
                Email = email,
                UserRole = UserRoles.Student,
                IsActive = true,
            };

            await _userManager.CreateAsync(userModel, password);

            await _db.SaveChangesAsync();
        }


        /// <summary>
        /// Sets the IsActive property of the logged-in user to false in the database.
        /// </summary>
        /// <param name="loggedUser"></param>
        public void Deactivate(AspNetUser loggedUser)
        {
            loggedUser.IsActive = false;

            _db.SaveChanges();
        }

        /// <summary>
        /// Returns a User object from the database based on the given user ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public AspNetUser? GetUserById(string userId)
        {
            var user = _db?.AspNetUsers?
                .Include(c => c.ChildrenRelations)
                .ThenInclude(c => c.Child)
                .Include(p => p.ParentRelations)
                .ThenInclude(p => p.Parent)
                .Include(s => s.SchoolClass)
                .FirstOrDefault(u => u.Id == Guid.Parse(userId));

            return user;
        }

        public void AddChild(Guid parentId, string childFirstName, string childLastName)
        {
            var child = _db?.Users?.FirstOrDefault(u => u.FirstName == childFirstName && u.LastName == childLastName);
            var parent = _db?.Users?.FirstOrDefault(u => u.Id == parentId);

            UserRelation userRelation = new UserRelation();
            userRelation.Child = child;
            userRelation.ChildId = child.Id;
            userRelation.Parent = parent;
            userRelation.ParentId = parent.Id;

            parent.ChildrenRelations.Add(userRelation);

            _db.SaveChanges();
        }

        public IEnumerable<AspNetUser> GetAllUsers()
        {
            return _db.AspNetUsers
                .Include(s => s.School)
                .Include(sc => sc.SchoolClass)
                .ToList();
        }

        public AspNetUser GetUserByNames(string firstLastName)
        {
            var firstName = firstLastName.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
            var lastName = firstLastName.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];

            var user = _db.Users.FirstOrDefault(u => u.FirstName == firstName && u.LastName == lastName);

            return user;
        }
    }
}
