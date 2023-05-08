using GradeCenter.Data.Models.Account;
using GradeCenter.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private UserManager<IdentityUser> _userManager;

        public AccountController(IAccountService accountService, UserManager<IdentityUser> userManager)
        {
            _accountService = accountService;
            _userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(string userName, string email, string password)
        {
            await _accountService.Register(userName, email, password);

            return Ok();
        }

        [HttpPost("Login")]
        public IActionResult Login(string userName, string password)
        {
            var token = _accountService.Login(userName, password);

            if (token == null || token == string.Empty)
                return BadRequest(new { message = "User name or password is incorrect" });

            return Ok(token);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(string newPassword, string newPhoneNumber)
        {
            User loggedUser = (User)await _userManager.FindByNameAsync(User.Identity.Name);

            if (loggedUser == null)
                return Unauthorized(new { message = "User must be authorized to perform this operation." });

            _accountService.UpdateUser(loggedUser, newPassword, newPhoneNumber);

            return Ok();
        }

        [HttpGet("IsLoggedIn")]
        public bool IsLoggedIn()
        {
            var user = User.Identity;

            return user.IsAuthenticated;
        }

        [HttpGet("GetLoggedUser")]
        public async Task<User> GetLoggedUser()
        {
            User loggedUser = (User)await _userManager.FindByNameAsync(User.Identity.Name);

            return loggedUser;
        }

        [HttpGet("GetUserById")]
        public async Task<User> GetUserById(string userId)
        {
            var user = _accountService.GetUserById(userId);

            return user;
        }
    }
}
