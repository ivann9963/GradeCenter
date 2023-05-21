using GradeCenter.API.Common;
using GradeCenter.API.Models.Request.SchoolRequests;
using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services.Schools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ISchoolService _schoolService;
        private readonly RequestValidator _requestValidator;
        private readonly ModelsFactory _modelsFactory;

        public SchoolController(UserManager<AspNetUser> userManager, ISchoolService schoolService)
        {
            _userManager = userManager;
            _schoolService = schoolService;
            _requestValidator = new RequestValidator(_userManager);
            _modelsFactory = new ModelsFactory();
        }
        /// <summary>
        /// Reads all the existing School entities in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllSchools")]
        public IActionResult GetAllSchools()
        {
            return Ok(_schoolService.GetAllSchools());
        }

        /// <summary>
        /// Creates an object of type School and stores it in the database.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(SchoolCreateRequest requestModel)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            School mappedSchoolModel = _modelsFactory.ExtractSchool(requestModel);

            await _schoolService.Create(mappedSchoolModel);

            return Ok();
        }

        /// <summary>
        /// Updates an object of type School and stores it in the database.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(SchoolUpdateRequest requestModel)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            School mappedSchoolModel = _modelsFactory.ExtractSchool(requestModel);

            await _schoolService.Update(mappedSchoolModel);

            return Ok();
        }

        [HttpPut("AddPrincipal")]
        public async Task<IActionResult> AddPrincipal(SchoolUpdateRequest requestModel)
        { 
            var loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);

            //var checkedRequest = await _requestValidator.ValidateRequest(ModelState);

            //if (checkedRequest != null)
            //return checkedRequest;

            School mappedSchoolModel = _modelsFactory.ExtractSchool(requestModel);

            await _schoolService.AddPrincipleToSchool(mappedSchoolModel);

            return Ok();
        }

        [HttpPut("AddTeachers")]
        public async Task<IActionResult> AddTeachers(SchoolUpdateRequest requestModel)
        {
            var loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);

            //var checkedRequest = await _requestValidator.ValidateRequest(ModelState);

            //if (checkedRequest != null)
            //return checkedRequest;

            School mappedSchoolModel = _modelsFactory.ExtractSchool(requestModel);

            await _schoolService.AddTeachersToSchool(mappedSchoolModel);

            return Ok();
        }

        [HttpPut("AddStudents")]
        public async Task<IActionResult> AddStudents(SchoolUpdateRequest requestModel)
        {
            var loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);

            //var checkedRequest = await _requestValidator.ValidateRequest(ModelState);

            //if (checkedRequest != null)
            //return checkedRequest;

            School mappedSchoolModel = _modelsFactory.ExtractSchool(requestModel);

            await _schoolService.AddStudentsToSchool(mappedSchoolModel);

            return Ok();
        }

        /// <summary>
        /// Soft deletes an object of type School in the database.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string name)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            await _schoolService.Delete(name);

            return Ok();
        }

        /// <summary>
        /// Creates an object of type SchoolClass in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("CreateClass")]
        public async Task<IActionResult> CreateClass(SchoolClassCreateRequest request)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            SchoolClass mappedSchoolClassModel = _modelsFactory.ExtractSchoolClass(request);

            await _schoolService.CreateClass(mappedSchoolClassModel);

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
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            await _schoolService.EnrollForClass(requestModel.SchoolClassId, requestModel.StudentId);

            return Ok();
        }

        /// <summary>
        /// Updates an object of type SchoolClass in order to withdraw
        /// an existing student.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPut("WithdrawFromClass")]
        public async Task<IActionResult> WithdrawFromClass(EnrollWithdrawRequestModel requestModel)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            await _schoolService.WithdrawFromClass(requestModel.SchoolClassId, requestModel.StudentId);

            return Ok();
        }
    }
}
