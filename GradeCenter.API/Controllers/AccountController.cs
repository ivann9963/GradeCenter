using GradeCenter.Data.Models.Account;
using GradeCenter.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private UserManager<User> _userManager;

        public AccountController(IAccountService accountService, UserManager<User> userManager)
        {
            _accountService = accountService;
            _userManager = userManager;
        }

        /// <summary>
        /// Creates an object of type User and stores it in the database.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public async Task<IActionResult> Register(string userName, string email, string password)
        {
            await _accountService.Register(userName, email, password);

            return Ok();
        }

        /// <summary>
        /// Checks if the user exists and returns a JWT token.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var token = await _accountService.Login(userName, password);

            if (token == null || token == string.Empty)
                return BadRequest(new { message = "User name or password is incorrect" });

            return Ok(token);
        }

        [HttpPut("AddChild")]
        public async Task<IActionResult> AddChild(Guid childId)
        {
            User parent = (User)await _userManager.FindByNameAsync(User.Identity.Name);

            if (parent == null)
                return Unauthorized(new { message = "User must be authorized to perform this operation." });

            _accountService.AddChild(parent, childId);

            return Ok();
        }

        /// <summary>
        /// Currently updates only password and phonenumber.
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="newPhoneNumber"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(string newPassword, string newPhoneNumber)
        {
            User loggedUser = (User)await _userManager.FindByNameAsync(User.Identity.Name);

            if (loggedUser == null)
                return Unauthorized(new { message = "User must be authorized to perform this operation." });

            _accountService.UpdateUser(loggedUser, newPassword, newPhoneNumber);

            return Ok();
        }

        /// <summary>
        /// Takes the JWT token and uses the userManager to find the corresponding object.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLoggedUser")]
        public async Task<User> GetLoggedUser()
        {
            User loggedUser = (User)await _userManager.FindByNameAsync(User.Identity.Name);

            return loggedUser;
        }

        /// <summary>
        /// Finds the concrete user from the database based on the UserId provided.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetUserById")]
        public async Task<User> GetUserById(string userId)
        {
            var user = _accountService.GetUserById(userId);

            return user;
        }
    }
}
