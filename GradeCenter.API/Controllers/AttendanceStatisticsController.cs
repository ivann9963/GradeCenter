using GradeCenter.Data.Models;
using GradeCenter.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceStatisticsController : ControllerBase
    {
        private readonly IAttendanceStatisticsService _attendanceStatisticsService;

        public AttendanceStatisticsController(IAttendanceStatisticsService attendanceStatisticsService)
        {
            _attendanceStatisticsService = attendanceStatisticsService;
        }

        [HttpGet("GetWeekly")]
        public List<Statistic> GetWeekly()
        {
            return _attendanceStatisticsService.GetWeekly();
        }

        [HttpGet("GetMonthly")]
        public List<Statistic> GetMonthly()
        {
            return _attendanceStatisticsService.GetMonthly();
        }

        [HttpGet("GetYearly")]
        public List<Statistic> GetYearly()
        {
            return _attendanceStatisticsService.GetYearly();
        }
    }
}
