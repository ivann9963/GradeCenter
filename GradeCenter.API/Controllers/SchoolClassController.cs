using GradeCenter.API.Common;
using GradeCenter.API.Models.Request.SchoolRequests;
using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolClassController : ControllerBase
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ISchoolClassService _schoolClassService;
        private readonly RequestValidator _requestValidator;
        private readonly ModelsFactory _modelsFactory;

        public SchoolClassController(UserManager<AspNetUser> userManager, ISchoolClassService schoolClassService)
        {
            _userManager = userManager;
            _schoolClassService = schoolClassService;
            _requestValidator = new RequestValidator(_userManager);
            _modelsFactory = new ModelsFactory();
        }

        /// <summary>
        /// Returns all schoolClasses in database
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllClassess")]
        public async Task<IActionResult> GetAllClassess()
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, UserRoles.Admin);

            if (checkedRequest != null)
                return checkedRequest;

            var allClassess = _schoolClassService.GetAllClassess();

            return Ok(allClassess);
        }

        [HttpGet("GetClassessInSchool")]
        public async Task<IActionResult> GetClassessInSchool(string schoolId)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, UserRoles.Admin);

            if (checkedRequest != null)
                return checkedRequest;

            List<SchoolClass> classessInSchool = _schoolClassService.GetClassessInSchool(schoolId);

            return Ok(classessInSchool);
        }

        /// <summary>
        /// Creates an object of type SchoolClass in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateClass")]
        public async Task<IActionResult> CreateClass(SchoolClassCreateRequest request)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, UserRoles.Admin);

            if (checkedRequest != null)
                return checkedRequest;

            SchoolClass mappedSchoolClassModel = _modelsFactory.ExtractSchoolClass(request);

            await _schoolClassService.CreateClass(mappedSchoolClassModel);

            return Ok();
        }

        /// <summary>
        /// Updates an object of type SchoolClass in order to enroll
        /// a new student.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPut("EnrollForClass")]
        public async Task<IActionResult> EnrollForClass(EnrollWithdrawRequestModel requestModel)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, UserRoles.Admin);

            if (checkedRequest != null)
                return checkedRequest;

            await _schoolClassService.EnrollForClass(requestModel.SchoolClassName, requestModel.StudentId);

            return Ok();
        }

        /// <summary>
        /// Updates an object of type SchoolClass in order to withdraw
        /// an existing student.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPut("WithdrawFromClass")]
        public async Task<IActionResult> WithdrawFromClass(string studentId)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, UserRoles.Admin);

            if (checkedRequest != null)
                return checkedRequest;

            await _schoolClassService.WithdrawFromClass(studentId);

            return Ok();
        }
    }
}
