using GradeCenter.Data.Models.Account;
using GradeCenter.Services.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private UserManager<AspNetUser> _userManager;
        private readonly RequestValidator _requestValidator;

        public AccountController(IAccountService accountService, UserManager<AspNetUser> userManager)
        {
            _accountService = accountService;
            _userManager = userManager;
            _requestValidator = new RequestValidator(_userManager);
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
        public async Task<IActionResult> AddChild(Guid parentId, string childFirstName, string childLastName)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            _accountService.AddChild(parentId, childFirstName, childFirstName);

            return Ok();
        }

        /// <summary>
        /// Currently updates only password and phonenumber.
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="newPhoneNumber"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(string? userId, string? newPassword, UserRoles? newRole, bool? isActive, string? newPhoneNumber)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            _accountService.UpdateUser(userId,newPassword, newRole, isActive, newPhoneNumber);

            return Ok();
        }

        /// <summary>
        /// Takes the JWT token and uses the userManager to find the corresponding object.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLoggedUser")]
        public async Task<AspNetUser> GetLoggedUser()
        {
            AspNetUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);

            return loggedUser;
        }

        /// <summary>
        /// Finds the concrete user from the database based on the UserId provided.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("GetUserById")]
        public async Task<AspNetUser> GetUserById(string userId)
        {
            var user = _accountService.GetUserById(userId);

            return user;
        }


        /// <summary>
        /// Get All users
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(_accountService.GetAllUsers());
        }
    }
}
