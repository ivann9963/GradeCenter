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

        [HttpGet("GetSchoolById")]
        public async Task<IActionResult> GetSchoolById(string schoolId)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            var school = _schoolService.GetSchoolById(schoolId);

            return Ok(school);
        }

        [HttpGet("GetPeopleInSchool")]
        public async Task<IActionResult> GetPeopleInSchool(string schoolId)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            List<AspNetUser> people = _schoolService.GetPeopleInSchool(schoolId);

            return Ok(people);
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
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            School mappedSchoolModel = _modelsFactory.ExtractSchool(requestModel);

            await _schoolService.AddPrincipleToSchool(mappedSchoolModel);

            return Ok();
        }

        [HttpPut("AddTeachers")]
        public async Task<IActionResult> AddTeachers(SchoolUpdateRequest requestModel)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            School mappedSchoolModel = _modelsFactory.ExtractSchool(requestModel);

            await _schoolService.AddTeachersToSchool(mappedSchoolModel);

            return Ok();
        }

        [HttpPut("AddStudents")]
        public async Task<IActionResult> AddStudents(SchoolUpdateRequest requestModel)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

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
    }
}
