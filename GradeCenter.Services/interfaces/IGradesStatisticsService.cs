using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services.interfaces
{
    public interface IGradesStatisticsService
    {
        List<Statistic> GetMonthly();
        
        List<Statistic> GetWeekly();

        List<Statistic> GetYearly();

        void CreateGradesStatistic(string? schoolId, string? schoolClassId, string? teacherId, string? disciplineName);
    }
}
