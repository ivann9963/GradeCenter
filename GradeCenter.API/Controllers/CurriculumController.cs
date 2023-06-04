using GradeCenter.API.Common;
using GradeCenter.API.Models.Request.CurricullumRequests;
using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services.interfaces;
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
        private readonly ModelsFactory _modelsFactory;

        public CurriculumController(ICurriculumService curriculumService, UserManager<AspNetUser> userManager)
        {
            this._curriculumService = curriculumService;
            _userManager = userManager;
            _requestValidator = new RequestValidator(_userManager);
            _modelsFactory = new ModelsFactory();
        }
        [HttpGet("GetDisciplineByTeacherId")]
        public async Task<IActionResult> GetDisciplineByTeacherId(string teacherId)
        {
            var teacher = _curriculumService.GetDisciplineByTeacherId(teacherId);

            return Ok(teacher);
        }
        /// <summary>
        /// Accepts a list of disciplines to create a new curriculum. 
        /// Validates the request before proceeding.
        /// </summary>
        /// <param name="curriculum"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(List<DisciplineDto> disciplines)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            var curricullum = _modelsFactory.ExtractCurricullum(disciplines);

            _curriculumService.Create(curricullum);

            return Ok();
        }

        /// <summary>
        /// Accepts a list of disciplines to update an existing curriculum. 
        /// The request is validated before the update.
        /// </summary>
        /// <param name="curricullum"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public async Task<IActionResult> Update(List<DisciplineDto> disciplines)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            var curricullum = _modelsFactory.ExtractCurricullum(disciplines);
            _curriculumService.Update(curricullum);

            return Ok();
        }

        /// <summary>
        /// Deletes the disciplines provided from the curriculum after validating the request.
        /// </summary>
        /// <param name="curricullum"></param>
        /// <returns></returns>
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<DisciplineDto> disciplines)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            var curricullum = _modelsFactory.ExtractCurricullum(disciplines);

            _curriculumService.Delete(curricullum);

            return Ok();
        }

        /// <summary>
        /// Retrieves the disciplines associated with the currently logged-in user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLoggedUserClasses")]
        public async Task<List<Discipline>?> GetLoggedUserClasses()
        {
            var loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (loggedUser == null)
                return null;

            var loggedUserClasses = _curriculumService.GetLoggedUserClasses(loggedUser.Id);

            return loggedUserClasses;
        }

        /// <summary>
        /// Gets the disciplines for a given school class and day.
        /// </summary>
        /// <param name="schoolClassId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        [HttpGet("GetClassesForDay")]
        public async Task<List<Discipline>?> GetClassesForDay(Guid schoolClassId, DayOfWeek day)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return null;

            var loggedUserClasses = _curriculumService.GetClassesForDay(schoolClassId, day);

            return loggedUserClasses;
        }

        /// <summary>
        /// Gets all disciplines for a given school class.
        /// </summary>
        /// <param name="schoolClassId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        [HttpGet("GetCurricullumForSchoolClass")]
        public async Task<List<Discipline>?> GetCurricullumForSchoolClass(Guid schoolClassId)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return null;

            var schoolClassCurricullum = _curriculumService.GetCurricullumForSchoolClass(schoolClassId);

            return schoolClassCurricullum;
        }

        [HttpGet("GetLoggedUserCurricullum")]
        public async Task<List<Discipline>?> GetLoggedUserCurricullum()
        {
            var loggedUser = await _requestValidator.GetLoggedUser(User);

            if (loggedUser == null)
                return null;

            var loggedUserCurricullum = _curriculumService.GetLoggedUserClasses(loggedUser.Id);

            return loggedUserCurricullum;
        }
    }
}
