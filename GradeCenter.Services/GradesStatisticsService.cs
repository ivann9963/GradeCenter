using GradeCenter.Data;
using GradeCenter.Data.Models;
using GradeCenter.Services.interfaces;

namespace GradeCenter.Services
{
    public class GradesStatisticsService : IGradesStatisticsService
    {
        private readonly GradeCenterContext _db;

        public GradesStatisticsService(GradeCenterContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get statistics since 1st, till the last day of current month.
        /// </summary>
        /// <returns></returns>
        public List<Statistic> GetMonthly()
        {
            var monthly = _db.Statistics
                .Where(s => s.CreatedOn.Month == DateTime.UtcNow.Month)
                .ToList();

            return monthly;
        }

        /// <summary>
        /// Get statistics since Monday to Sunday
        /// </summary>
        /// <returns></returns>
        public List<Statistic> GetWeekly()
        {
            var weekly = _db.Statistics
             .Where(s => s.CreatedOn.Day >= ((int)DayOfWeek.Monday) && s.CreatedOn.Day <= ((int)DayOfWeek.Sunday))
             .ToList();

            return weekly;
        }

        /// <summary>
        /// Get statistic since JAN 1 till last day of the current year.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<Statistic> GetYearly()
        {
            var yearly = _db.Statistics
               .Where(s => s.CreatedOn.Year == DateTime.UtcNow.Year)
               .ToList();

            return yearly;
        }

        /// <summary>
        /// Creates a statistic based on the provided parameters.
        /// </summary>
        /// <param name="school"></param>
        /// <param name="schoolClass"></param>
        /// <param name="teacher"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void CreateGradesStatistic(string? schoolId, string? schoolClassId, string? teacherId, string? disciplineName)
        {
            Statistic statistic = ExtractAverageRate(schoolId, schoolClassId, teacherId, disciplineName);
            statistic.ComparedToLastWeek = ExtractComparedToLastWeek(statistic);
            statistic.ComparedToLastMonth = ExtractComparedToLastMonth(statistic);
            statistic.ComparedToLastYear = ExtractComparedToLastYear(statistic);

            statistic.CreatedOn = DateTime.UtcNow;
            statistic.StatisticType = StatisticTypes.Grades;

            _db.Statistics.Add(statistic);
            _db.SaveChanges();
        }

        private double ExtractComparedToLastYear(Statistic statistic)
        {
            var (startOfThisYear, endOfThisYear) = GetYearBoundaries(DateTime.Today);
            double currentYearStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfThisYear && x.CreatedOn <= endOfThisYear).Average(x => x.AverageRate);

            var (startOfLastYear, endOfLastYear) = GetYearBoundaries(DateTime.Today.AddYears(-1));
            double lastYearStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfLastYear && x.CreatedOn <= endOfLastYear).Average(x => x.AverageRate);

            double difference = currentYearStatistics - lastYearStatistics;
            double percentageDifference = (difference / lastYearStatistics) * 100;

            return percentageDifference;
        }

        public static (DateTime StartOfYear, DateTime EndOfYear) GetYearBoundaries(DateTime date)
        {
            DateTime startOfYear = new DateTime(date.Year, 1, 1);
            DateTime endOfYear = new DateTime(date.Year, 12, 31);

            return (startOfYear, endOfYear);
        }

        private double ExtractComparedToLastMonth(Statistic statistic)
        {
            var (startOfThisMonth, endOfThisMonth) = GetMonthBoundaries(DateTime.Today);
            double currentMonthStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfThisMonth && x.CreatedOn <= endOfThisMonth).Average(x => x.AverageRate);

            var (startOfLastMonth, endOfLastMonth) = GetMonthBoundaries(DateTime.Today.AddMonths(-1));
            double lastMonthStatistics = _db.Statistics.Where(x => x.CreatedOn > startOfLastMonth && x.CreatedOn <= endOfLastMonth).Average(x => x.AverageRate);

            double difference = currentMonthStatistics - lastMonthStatistics;
            double percentageDifference = (difference / lastMonthStatistics) * 100;

            return percentageDifference;
        }

        public static (DateTime StartOfMonth, DateTime EndOfMonth) GetMonthBoundaries(DateTime date)
        {
            DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            return (startOfMonth, endOfMonth);
        }

        private double ExtractComparedToLastWeek(Statistic statistic)
        {
            var (startOfLastWeek, endOfLastWeek) = GetWeekBoundaries(DateTime.Today.AddDays(-7));
            double lastWeekStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfLastWeek && x.CreatedOn <= endOfLastWeek).Average(x => x.AverageRate);

            var (startOfWeek, endOfWeek) = GetWeekBoundaries(DateTime.Today);
            double currentWeekStatistics = _db.Statistics.Where(x => x.CreatedOn > startOfWeek && x.CreatedOn <= endOfWeek).Average(x => x.AverageRate);

            double difference = currentWeekStatistics - lastWeekStatistics;
            double percentageDifference = (difference / lastWeekStatistics) * 100;

            return percentageDifference;
        }

        public static (DateTime StartOfWeek, DateTime EndOfWeek) GetWeekBoundaries(DateTime date)
        {
            int diffStart = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = date.AddDays(-diffStart);

            int diffEnd = (7 + (DayOfWeek.Friday - date.DayOfWeek)) % 7;
            DateTime endOfWeek = date.AddDays(diffEnd);

            return (startOfWeek, endOfWeek);
        }


        private Statistic ExtractAverageRate(string? schoolId, string? schoolClassId, string? teacherId, string? disciplineName)
        {
            Statistic statistic = new Statistic();

            statistic.AverageSchoolRate = AvgSchoolGrade(schoolId, disciplineName);
            statistic.School = _db.Schools.FirstOrDefault(x => x.Id == schoolId);

            statistic.AverageSchoolClassRate = AvgClassGrade(schoolClassId, disciplineName);
            statistic.SchoolClass = _db.SchoolClasses.FirstOrDefault(x => x.Id == Guid.Parse(schoolClassId));

            statistic.AverageTeacherRate = AvgTeacherGrade(teacherId);
            statistic.Teacher = _db.AspNetUsers.FirstOrDefault(x => x.Id == Guid.Parse(teacherId));

            return statistic;
        }

        private double AvgTeacherGrade(string teacherId)
        {
            var teacher = _db.AspNetUsers.FirstOrDefault(u => u.Id == Guid.Parse(teacherId));

            var avgGrade = _db.Disciplines.Where(d => d.TeacherId == Guid.Parse(teacherId)).Average(x => x.Grades.Average(r => r.Rate));

            return avgGrade;
        }

        private double AvgClassGrade(string schoolClassId, string? disciplineName)
        {
            var schoolClass = _db.SchoolClasses.FirstOrDefault(c => c.Id == Guid.Parse(schoolClassId));

            var avgGrade = schoolClass.Curriculum.SelectMany(g => g.Grades).ToList().Average(s => s.Rate);

            return avgGrade;
        }

        private double AvgSchoolGrade(string? schoolId, string? disciplineName)
        {
            var school = _db.Schools.FirstOrDefault(s => s.Id == schoolId);

            var avgGrade = school.SchoolClasses.SelectMany(g => g.Curriculum.Where(d => d.Name == disciplineName).SelectMany(g => g.Grades))
                .ToList().Average(x => x.Rate);

            return avgGrade;
        }
    }
}
