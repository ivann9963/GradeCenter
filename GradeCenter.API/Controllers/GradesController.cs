using GradeCenter.API.Common;
using GradeCenter.API.Models.Request.GradeRequests;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services.Grades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly RequestValidator _requestValidator;
        private readonly ModelsFactory _modelsFactory;

        public GradesController(UserManager<AspNetUser> userManager, IGradeService gradeService)
        {
            _userManager = userManager;
            _gradeService = gradeService;
            _requestValidator = new RequestValidator(_userManager);
            _modelsFactory = new ModelsFactory();
        }

        /// <summary>
        /// Returns all grades in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllGrades")]
        public async Task<IActionResult> GetAllGradesAsync()
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, new List<UserRoles> { UserRoles.Admin, UserRoles.Principle, UserRoles.Student, UserRoles.Teacher });

            if (checkedRequest != null)
                return checkedRequest;

            var allGrades = _gradeService.GetAllGrades();

            return Ok(allGrades);
        }

        /// <summary>
        /// Creates an object of type Grade in the database.
        /// </summary>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody]GradeRequestModel requestModel)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, new List<UserRoles> { UserRoles.Admin, UserRoles.Teacher });

            if (checkedRequest != null)
                return checkedRequest;

            var mappedGradeModel = _modelsFactory.ExtractGrade(requestModel);

            await _gradeService.Create(mappedGradeModel);

            return Ok();
        }

        /// <summary>
        /// Updates an object of type Grade in the database.
        /// </summary>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody]GradeRequestModel requestModel)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, new List<UserRoles> { UserRoles.Admin, UserRoles.Teacher });

            if (checkedRequest != null)
                return checkedRequest;

            var mappedGradeModel = _modelsFactory.ExtractGrade(requestModel);

            await _gradeService.Update(mappedGradeModel);

            return Ok();
        }

        /// <summary>
        /// Delete an object of type Grade in the database.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, new List<UserRoles> { UserRoles.Admin, UserRoles.Teacher });

            if (checkedRequest != null)
                return checkedRequest;

            await _gradeService.Delete(id);

            return Ok();
        }
    }
}
