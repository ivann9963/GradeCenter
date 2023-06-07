using GradeCenter.Data.Models;

namespace GradeCenter.Services.interfaces
{
    public interface IStatisticsService
    {
        List<Statistic> GetMonthly(StatisticTypes statisticType);

        List<Statistic> GetWeekly(StatisticTypes statisticType);

        List<Statistic> GetYearly(StatisticTypes statisticType);

        List<Statistic> GetSchoolStatistics();

        List<Statistic> GetClassStatistics();

        void CreateStatistic(string? schoolId, string? schoolClassId, string? teacherId, string? disciplineName, StatisticTypes statisticType);
    }
}
