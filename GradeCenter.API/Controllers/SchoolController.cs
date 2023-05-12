using AutoMapper;
using GradeCenter.API.Models.Request;
using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services.Schools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolService _schoolService;
        private readonly IMapper _mapper;

        public SchoolController(ISchoolService schoolService, IMapper mapper)
        {
            _schoolService = schoolService;
            _mapper = mapper;
        }
        /// <summary>
        /// Reads all the existing School entities in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Read")]
        public async Task<IActionResult> Read()
        {
            return Ok(_schoolService.Read());
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
            if (this.User.Identity.IsAuthenticated) return Unauthorized();

            if (!this.ModelState.IsValid) return BadRequest(ModelState);

            var mappedSchoolModel = _mapper
                    .Map<School>(requestModel);

            await _schoolService.Create(mappedSchoolModel);

            return Ok();
        }

        /// <summary>
        /// Updates an object of type School and stores it in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(int id, SchoolUpdateRequestModel requestModel)
        {
            if (this.User.Identity.IsAuthenticated) return Unauthorized();

            if (!this.ModelState.IsValid) return BadRequest(ModelState);

            var mappedSchoolModel = _mapper
                    .Map<School>(requestModel);

            var mappedStudentModels = _mapper
                    .ProjectTo<User>(requestModel.Students.AsQueryable())
                    .ToList();
            var mappedTeacherModels = _mapper
                    .ProjectTo<User>(requestModel.Teachers.AsQueryable())
                    .ToList();
            var mappedPrincipalModel = _mapper
                    .Map<User>(requestModel.Principal);

            await _schoolService.Update(id, mappedSchoolModel, mappedStudentModels, mappedTeacherModels, mappedPrincipalModel);

            return Ok();
        }

        /// <summary>
        /// Soft deletes an object of type School in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (this.User.Identity.IsAuthenticated) return Unauthorized();

            if (!this.ModelState.IsValid) return BadRequest(ModelState);

            await _schoolService.Delete(id);

            return Ok();
        }
    }
}
