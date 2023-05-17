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
        private readonly RequestValidator _requestValidator;

        public CurriculumController(ICurriculumService curriculumService, UserManager<AspNetUser> userManager)
        {
            this._curriculumService = curriculumService;
            _userManager = userManager;
            _requestValidator = new RequestValidator(_userManager, User);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(List<Discipline> curriculum)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState);

            if (checkedRequest != null)
                return checkedRequest;

            _curriculumService.Create(curriculum);

            return Ok();
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(List<Discipline> curricullum)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState);

            if (checkedRequest != null)
                return checkedRequest;

            _curriculumService.Update(curricullum);

            return Ok();
        }


        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<Discipline> curricullum)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState);

            if (checkedRequest != null)
                return checkedRequest;

            _curriculumService.Delete(curricullum);

            return Ok();
        }

        [HttpGet("GetLoggedUserClasses")]
        public async Task<List<Discipline>?> GetLoggedUserClasses()
        {
            var loggedUser = await _requestValidator.GetLoggedUser();

            if (loggedUser == null)
                return null;

            var loggedUserClasses = _curriculumService.GetLoggedUserClasses(loggedUser.Id);

            return loggedUserClasses;
        }

        [HttpGet("GetClassesForDay")]
        public async Task<List<Discipline>?> GetClassesForDay(Guid schoolClassId, DayOfWeek day)
        {
            var loggedUser = await _requestValidator.GetLoggedUser();

            if (loggedUser == null)
                return null;

            var loggedUserClasses = _curriculumService.GetClassesForDay(schoolClassId, day);

            return loggedUserClasses;
        }
    }
}
