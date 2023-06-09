using GradeCenter.API.Common;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services;
using GradeCenter.Services.Attendances;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRelationsController : ControllerBase
    {
        private readonly IUserRelationService _userRelationService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly RequestValidator _requestValidator;

        public UserRelationsController(UserManager<AspNetUser> userManager, IUserRelationService userRelationService)
        {
            _userManager = userManager;
            _userRelationService = userRelationService;
            _requestValidator = new RequestValidator(_userManager);
        }

        /// <summary>
        /// Returns all user relations in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllUserRelations")]
        public async Task<IActionResult> GetAllUserRelations()
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, new List<UserRoles> { UserRoles.Admin, UserRoles.Principle, UserRoles.Student, UserRoles.Teacher, UserRoles.Parent });

            if (checkedRequest != null)
                return checkedRequest;

            var userRelations = _userRelationService.GetAll();

            return Ok(userRelations);
        }

        /// <summary>
        /// Returns all user relations by student name in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllUserRelationsByStudentName")]
        public async Task<IActionResult> GetAllUserRelationsByStudentName(string studentName)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, new List<UserRoles> { UserRoles.Admin, UserRoles.Principle, UserRoles.Student, UserRoles.Teacher, UserRoles.Parent });

            if (checkedRequest != null)
                return checkedRequest;

            var userRelations = _userRelationService.GetByStudentName(studentName);

            return Ok(userRelations);
        }

        /// <summary>
        /// Returns all user relations by student name in the database.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllUserRelationsByParentName")]
        public async Task<IActionResult> GetAllUserRelationsByParentName(string parentName)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, new List<UserRoles> { UserRoles.Admin, UserRoles.Principle, UserRoles.Student, UserRoles.Teacher, UserRoles.Parent });

            if (checkedRequest != null)
                return checkedRequest;

            var userRelations = _userRelationService.GetByStudentName(parentName);

            return Ok(userRelations);
        }
    }
}
