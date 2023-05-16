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

        public SchoolController(UserManager<AspNetUser> userManager, ISchoolService schoolService)
        {
            _userManager = userManager;
            _schoolService = schoolService;
        }
        /// <summary>
        /// Reads all the existing School entities in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllSchools")]
        public async Task<IActionResult> GetAllSchools()
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
            var checkedReqeust = await ValidateRequest();

            if (checkedReqeust != null)
                return checkedReqeust;

            School mappedSchoolModel = ExtractSchool(requestModel);

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
            var checkedReqeust = await ValidateRequest();

            if (checkedReqeust != null)
                return checkedReqeust;

            School mappedSchoolModel = ExtractSchool(requestModel);

            await _schoolService.Update(mappedSchoolModel);

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
            var checkedReqeust = await ValidateRequest();

            if (checkedReqeust != null)
                return checkedReqeust;

            await _schoolService.Delete(name);

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

        private static School ExtractSchool(SchoolCreateRequest requestModel)
        {
            var model = requestModel;
            List<AspNetUser> users = new List<AspNetUser>();

            if (requestModel is SchoolUpdateRequest updateRequest 
                && updateRequest.Users != null && updateRequest.Users.Count >= 0)
            {
                model = updateRequest;
                users = updateRequest.Users.Select(x => new AspNetUser { Id = x.UserId }).ToList();
            }

            return new School
            {
                Id = model is SchoolUpdateRequest updateModel ? updateModel.Id : null,
                Name = model.Name,
                Address = model.Address,
                People = users
            };
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
