using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurriculumController : ControllerBase
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ICurriculumService _curriculumService;

        public CurriculumController(ICurriculumService curriculumService, UserManager<AspNetUser> userManager)
        {
            this._curriculumService = curriculumService;
            _userManager = userManager;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(List<Discipline> curriculum, SchoolClass schoolClass)
        {
            var checkedReqeust = await ValidateRequest();

            if (checkedReqeust != null)
                return checkedReqeust;

            _curriculumService.Create(curriculum, schoolClass);

            return Ok();
        }

        private async Task<IActionResult> ValidateRequest()
        {
            var loggedUser = await GetLoggedUser();

            if (loggedUser == null || !IsAdmin(loggedUser))
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest("Invalid model state.");

            return null;
        }

        private async Task<AspNetUser> GetLoggedUser()
        {
            return await _userManager.FindByNameAsync(User.Identity.Name);
        }

        private bool IsAdmin(AspNetUser user)
        {
            return user.UserRole.Equals(UserRoles.Admin);
        }
    }
}
