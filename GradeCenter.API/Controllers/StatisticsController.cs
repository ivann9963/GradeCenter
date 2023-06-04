using GradeCenter.Data.Models;
using GradeCenter.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GradeCenter.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("GetWeekly")]
        public List<Statistic> GetWeekly(StatisticTypes statisticType)
        {
            return _statisticsService.GetWeekly(statisticType);
        }

        [HttpGet("GetMonthly")]
        public List<Statistic> GetMonthly(StatisticTypes statisticType)
        {
            return _statisticsService.GetMonthly(statisticType);
        }

        [HttpGet("GetYearly")]
        public List<Statistic> GetYearly(StatisticTypes statisticType)
        {
            return _statisticsService.GetYearly(statisticType);
        }

        /// <summary>
        /// Creates statistics based either for a School, School Class or a Teacher.
        /// The type can be either for Attendances or Grades.
        /// The Statistic has AverageRate (for Grades or Attendance) for either School, School Class or a Teacher.
        /// It also has This week AverageRate compared to last week's average rate. 
        /// Similerly it compares this month to last month and this year to last year.
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="discipline"></param>
        [HttpPost]
        public void CreateGradesStatistics(string schoolId, string schoolClassIdId, string? teacherId, string discipline, StatisticTypes statisticType)
        {
            _statisticsService.CreateStatistic(schoolId, schoolClassIdId, teacherId, discipline, statisticType);
        }
    }
}
