using GradeCenter.Data.Models;
using GradeCenter.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesStatisticsController : ControllerBase
    {
        private readonly IGradesStatisticsService _gradesStatisticsService;

        public GradesStatisticsController(IGradesStatisticsService gradesStatisticsService)
        {
            _gradesStatisticsService = gradesStatisticsService;
        }

        [HttpGet("GetWeekly")]
        public List<Statistic> GetWeekly()
        {
            return _gradesStatisticsService.GetWeekly();
        }

        [HttpGet("GetMonthly")]
        public List<Statistic> GetMonthly()
        {
            return _gradesStatisticsService.GetMonthly();
        }

        [HttpGet("GetYearly")]
        public List<Statistic> GetYearly()
        {
            return _gradesStatisticsService.GetYearly();
        }

        /// <summary>
        /// Creates a statistic for the whole school for a conrete discipline.
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="discipline"></param>
        [HttpPost]
        public void CreateGradesStatistics(string schoolId, string schoolClassIdId, string? teacherId, string discipline)
        {
            _gradesStatisticsService.CreateGradesStatistic(schoolId, schoolClassIdId, teacherId, discipline);
        }
    }
}
