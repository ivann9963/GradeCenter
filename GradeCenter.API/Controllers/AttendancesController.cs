using GradeCenter.API.Common;
using GradeCenter.API.Models.Request.AttendanceRequests;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services.Attendances;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly RequestValidator _requestValidator;
        private readonly ModelsFactory _modelsFactory;

        public AttendancesController(UserManager<AspNetUser> userManager, IAttendanceService attendanceService)
        {
            _userManager = userManager;
            _attendanceService = attendanceService;
            _requestValidator = new RequestValidator(_userManager);
            _modelsFactory = new ModelsFactory();
        }

        /// <summary>
        /// Creates Attendance Object in the database
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody]AttendanceRequestModel requestModel)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, new List<UserRoles>() { UserRoles.Admin, UserRoles.Teacher });

            if (checkedRequest != null)
                return checkedRequest;

            var mappedAttendanceModel = _modelsFactory.ExtractAttendance(requestModel);

            await _attendanceService.Create(mappedAttendanceModel);

            return Ok();
        }

        /// <summary>
        /// Returns all Attendances from the database
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllAttendances")]
        public async Task<IActionResult> GetAllAttendancesAsync()
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User, new List<UserRoles> { UserRoles.Admin, UserRoles.Principle, UserRoles.Student, UserRoles.Teacher });

            if (checkedRequest != null)
                return checkedRequest;

            var allAttendances = _attendanceService.GetAllAttendances();

            return Ok(allAttendances);
        }

        /// <summary>
        /// Updates an object of type Attendance in the database.
        /// </summary>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(AttendanceRequestModel requestModel)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            var mappedAttendanceModel = _modelsFactory.ExtractAttendance(requestModel);

            await _attendanceService.Update(mappedAttendanceModel);

            return Ok();
        }

        /// <summary>
        /// Delete an object of type Attendance in the database.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            await _attendanceService.Delete(id);

            return Ok();
        }
    }
}
