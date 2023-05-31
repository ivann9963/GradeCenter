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
        /// Returns all attendances from the database
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllAttendances")]
        public async Task<IActionResult> GetAllAttendancesAsync()
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            var allGrades = _attendanceService.GetAllAttendances();

            return Ok(allGrades);
        }

        /// <summary>
        /// Creates Attendance Object in the database
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public async Task<IActionResult> Create(AttendanceRequestModel requestModel)
        {
            var checkedRequest = await _requestValidator.ValidateRequest(ModelState, User);

            if (checkedRequest != null)
                return checkedRequest;

            var mappedAttendanceModel = _modelsFactory.ExtractAttendance(requestModel);

            await _attendanceService.Create(mappedAttendanceModel);

            return Ok();
        }
    }
}
