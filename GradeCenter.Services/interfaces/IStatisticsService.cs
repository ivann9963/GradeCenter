using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services.interfaces
{
    public interface IStatisticsService
    {
        List<Statistic> GetMonthly();
        
        List<Statistic> GetWeekly();

        List<Statistic> GetYearly();
        void CreateGradesStatistic(string? schoolId, string? schoolClass, string? teacherId, string? discipline);
    }
}
