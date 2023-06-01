using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services.interfaces
{
    public interface IStatisticsService
    {
        List<Statistic> GetMonthly();
        
        List<Statistic> GetWeekly();

        List<Statistic> GetYearly();
        void CreateStatistic(School? school, SchoolClassService? schoolClass, AspNetUser? teacher, Discipline? discipline);
    }
}
