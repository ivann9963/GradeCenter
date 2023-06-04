using GradeCenter.Data;
using GradeCenter.Services.interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

        /// <summary>
        /// Creates a statistic for the whole school for a conrete discipline.
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="discipline"></param>
        [HttpPost]
        public void Create(string schoolId, string discipline)
        {
            _statisticsService.CreateGradesStatistic(schoolId, discipline, null, null);
        }
    }
}
