using GradeCenter.Data.Models;

namespace GradeCenter.Services.interfaces
{
    public interface IAttendanceStatisticsService
    {
        List<Statistic> GetMonthly();

        List<Statistic> GetWeekly();

        List<Statistic> GetYearly();

        void CreateAttendanceStatistic(string? schoolId, string? schoolClassId, string? teacherId, string? disciplineName);
    }
}
