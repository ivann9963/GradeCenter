using GradeCenter.API.Models.Request;
using GradeCenter.Services.Schools;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolService _schoolService;

        public SchoolController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        /// <summary>
        /// Creates an object of type School and stores it in the database.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(SchoolCreateRequestModel requestModel)
        {
            await _schoolService.Create(requestModel.Name, requestModel.Address, requestModel.PrincipalId, requestModel.PrincipalFirstName, requestModel.PrincipalLastName);

            return Ok();
        }

        /// <summary>
        /// Updates an object of type School and stores it in the database.
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(SchoolUpdateRequestModel requestModel)
        {
            await _schoolService.Update(requestModel.Id, requestModel.Name, requestModel.Address);

            return Ok();
        }

        /// <summary>
        /// Soft deletes an object of type School in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _schoolService.Delete(id);

            return Ok();
        }
    }
}
