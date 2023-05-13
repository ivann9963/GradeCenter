using GradeCenter.API.Common;
using GradeCenter.API.Models.Request;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISchoolService _schoolService;

        public SchoolController(UserManager<IdentityUser> userManager, ISchoolService schoolService)
        {
            _userManager = userManager;
            _schoolService = schoolService;
        }
        /// <summary>
        /// Reads all the existing School entities in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Read")]
        public async Task<IActionResult> Read()
        {
            return Ok(_schoolService.GetAllSchools());
        }

        /// <summary>
        /// Creates an object of type School and stores it in the database.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(SchoolCreateRequestModel requestModel)
        {
            User loggedUser = (User)await _userManager.FindByNameAsync(User.Identity.Name);

            if (!loggedUser.UserRole.Equals(UserRoles.Admin))
                return Unauthorized();

            if (!this.ModelState.IsValid)
                return BadRequest(ModelState);

            var mappedSchoolModel = FactoryBuilder.ToObject<School>(requestModel);

            await _schoolService.Create(mappedSchoolModel);

            return Ok();
        }

        /// <summary>
        /// Updates an object of type School and stores it in the database.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(SchoolUpdateRequestModel requestModel)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            User loggedUser = (User)await _userManager.FindByNameAsync(User.Identity.Name);

            if (!loggedUser.UserRole.Equals(UserRoles.Admin))
                return Unauthorized();

            if (!this.ModelState.IsValid)
                return BadRequest(ModelState);

            var mappedSchoolModel = FactoryBuilder.ToObject<School>(requestModel);
            var mappedUserModels = FactoryBuilder.ToObject<List<User>>(requestModel.Users);
            mappedSchoolModel.Users ??= mappedUserModels;

            await _schoolService.Update(mappedSchoolModel);

            return Ok();
        }

        /// <summary>
        /// Soft deletes an object of type School in the database.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string name)
        {
            User loggedUser = (User)await _userManager.FindByNameAsync(User.Identity.Name);

            if (!loggedUser.UserRole.Equals(UserRoles.Admin))
                return Unauthorized();

            if (!this.ModelState.IsValid)
                return BadRequest(ModelState);

            await _schoolService.Delete(name);

            return Ok();
        }
    }
}
