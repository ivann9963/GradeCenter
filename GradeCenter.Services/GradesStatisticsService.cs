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
                .Where(s => s.CreatedOn.Month == DateTime.UtcNow.Month
                    && s.StatisticType == StatisticTypes.Grades)
                .ToList();

            return monthly;
        }

        /// <summary>
        /// Get statistics since Monday to Sunday
        /// </summary>
        /// <returns></returns>
        public List<Statistic> GetWeekly()
        {
            var (startOfWeek, endOfWeek) = GetWeekBoundaries(DateTime.Today);

            var weekly = _db.Statistics
             .Where(s => s.CreatedOn >= startOfWeek && s.CreatedOn <= endOfWeek
                && s.StatisticType == StatisticTypes.Grades)
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
               .Where(s => s.CreatedOn.Year == DateTime.UtcNow.Year 
                    && s.StatisticType == StatisticTypes.Grades)
               .ToList();

            return yearly;
        }

        /// <summary>
        /// Creates a statistic based on the provided parameters.
        /// One Statistic at a time for either one of the options - School, School Class or a Teacher.
        /// AverageRate is the average of the grades for the selected option.
        /// ComparedToLastWeek compares the AverageRate of last week to the current week. (Same for LastMonth and Year)
        /// </summary>
        /// <param name="school"></param>
        /// <param name="schoolClass"></param>
        /// <param name="teacher"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void CreateGradesStatistic(string? schoolId, string? schoolClassId, string? teacherId, string? disciplineName)
        {
            Statistic statistic = new();
            statistic.AverageRate = ExtractAverageRate(schoolId, schoolClassId, teacherId, disciplineName);
            statistic.ComparedToLastWeek = ExtractComparedToLastWeek();
            statistic.ComparedToLastMonth = ExtractComparedToLastMonth();
            statistic.ComparedToLastYear = ExtractComparedToLastYear();

            statistic = SetSelectOption(schoolId, schoolClassId, teacherId);

            statistic.CreatedOn = DateTime.UtcNow;
            statistic.StatisticType = StatisticTypes.Grades;

            _db.Statistics.Add(statistic);
            _db.SaveChanges();
        }

        /// <summary>
        /// Checks which one of the options is NOT null.
        /// Fetches the information about the selected option from the DB.
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="schoolClassId"></param>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        private Statistic SetSelectOption(string? schoolId, string? schoolClassId, string? teacherId)
        {
            Statistic statistic = new Statistic();

            if (schoolId != null)
                statistic.School = _db.Schools.FirstOrDefault(x => x.Id == schoolId);
            else if (schoolClassId != null)
                statistic.SchoolClass = _db.SchoolClasses.FirstOrDefault(x => x.Id == Guid.Parse(schoolClassId));
            else if (teacherId != null)
                statistic.Teacher = _db.AspNetUsers.FirstOrDefault(x => x.Id == Guid.Parse(teacherId));

            return statistic;
        }

        /// <summary>
        /// Checks which is the selected option. (School, SchoolClass or Teacher)
        /// Calculates the average of the grades for the given option.
        /// Only 1 option will be set and the others will always be null.
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="schoolClassId"></param>
        /// <param name="teacherId"></param>
        /// <param name="disciplineName"></param>
        /// <returns></returns>
        private double ExtractAverageRate(string? schoolId, string? schoolClassId, string? teacherId, string? disciplineName)
        {
            double averageRate = 0;

            if (schoolId != null)
                averageRate = AvgSchoolGrade(schoolId, disciplineName);
            else if (schoolClassId != null)
                averageRate = AvgClassGrade(schoolClassId, disciplineName);
            else if (teacherId != null)
                averageRate = AvgTeacherGrade(teacherId);

            return averageRate;
        }

        /// <summary>
        /// Gets all grades of disciplines where 
        /// the owner is the given teacher and
        /// returns their average rate.
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        private double AvgTeacherGrade(string teacherId)
        {
            var teacher = _db.AspNetUsers.FirstOrDefault(u => u.Id == Guid.Parse(teacherId));

            var avgGrade = _db.Disciplines.Where(d => d.TeacherId == Guid.Parse(teacherId)).Average(x => x.Grades.Average(r => r.Rate));

            return avgGrade;
        }

        /// <summary>
        /// Gets all grades associated with the SchoolClass's Curricullum
        /// and returns their average.
        /// </summary>
        /// <param name="schoolClassId"></param>
        /// <param name="disciplineName"></param>
        /// <returns></returns>
        private double AvgClassGrade(string schoolClassId, string? disciplineName)
        {
            var schoolClass = _db.SchoolClasses.FirstOrDefault(c => c.Id == Guid.Parse(schoolClassId));

            var avgGrade = schoolClass.Curriculum.SelectMany(g => g.Grades).ToList().Average(s => s.Rate);

            return avgGrade;
        }

        /// <summary>
        /// Gets all grades associated with the given school
        /// and calculates their average.
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="disciplineName"></param>
        /// <returns></returns>
        private double AvgSchoolGrade(string? schoolId, string? disciplineName)
        {
            var school = _db.Schools.FirstOrDefault(s => s.Id == schoolId);

            var avgGrade = school.SchoolClasses.SelectMany(g => g.Curriculum.Where(d => d.Name == disciplineName).SelectMany(g => g.Grades))
                .ToList().Average(x => x.Rate);

            return avgGrade;
        }

        /// <summary>
        /// Compares this year's Average of all AverageRates for Grades
        /// to the last years.
        /// </summary>
        /// <returns></returns>
        private double ExtractComparedToLastYear()
        {
            var (startOfThisYear, endOfThisYear) = GetYearBoundaries(DateTime.Today);
            double currentYearStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfThisYear && x.CreatedOn <= endOfThisYear).Average(x => x.AverageRate);

            var (startOfLastYear, endOfLastYear) = GetYearBoundaries(DateTime.Today.AddYears(-1));
            double lastYearStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfLastYear && x.CreatedOn <= endOfLastYear).Average(x => x.AverageRate);

            double difference = currentYearStatistics - lastYearStatistics;
            double percentageDifference = (difference / lastYearStatistics) * 100;

            return percentageDifference;
        }

        /// <summary>
        /// Returns a touple of the first and last date of the year.
        /// It can be used to get the values for other years as well.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static (DateTime StartOfYear, DateTime EndOfYear) GetYearBoundaries(DateTime date)
        {
            DateTime startOfYear = new DateTime(date.Year, 1, 1);
            DateTime endOfYear = new DateTime(date.Year, 12, 31);

            return (startOfYear, endOfYear);
        }

        /// <summary>
        /// Compares this months average value of all AverageRates
        /// to the last months
        /// </summary>
        /// <returns></returns>
        private double ExtractComparedToLastMonth()
        {
            var (startOfThisMonth, endOfThisMonth) = GetMonthBoundaries(DateTime.Today);
            double currentMonthStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfThisMonth && x.CreatedOn <= endOfThisMonth).Average(x => x.AverageRate);

            var (startOfLastMonth, endOfLastMonth) = GetMonthBoundaries(DateTime.Today.AddMonths(-1));
            double lastMonthStatistics = _db.Statistics.Where(x => x.CreatedOn > startOfLastMonth && x.CreatedOn <= endOfLastMonth).Average(x => x.AverageRate);

            double difference = currentMonthStatistics - lastMonthStatistics;
            double percentageDifference = (difference / lastMonthStatistics) * 100;

            return percentageDifference;
        }

        /// <summary>
        /// Returns the first and last date of the selected month.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static (DateTime StartOfMonth, DateTime EndOfMonth) GetMonthBoundaries(DateTime date)
        {
            DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            return (startOfMonth, endOfMonth);
        }

        /// <summary>
        /// Compares this weeks average of all AverageRates
        /// to the last months
        /// </summary>
        /// <returns></returns>
        private double ExtractComparedToLastWeek()
        {
            var (startOfLastWeek, endOfLastWeek) = GetWeekBoundaries(DateTime.Today.AddDays(-7));
            double lastWeekStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfLastWeek && x.CreatedOn <= endOfLastWeek).Average(x => x.AverageRate);

            var (startOfWeek, endOfWeek) = GetWeekBoundaries(DateTime.Today);
            double currentWeekStatistics = _db.Statistics.Where(x => x.CreatedOn > startOfWeek && x.CreatedOn <= endOfWeek).Average(x => x.AverageRate);

            double difference = currentWeekStatistics - lastWeekStatistics;
            double percentageDifference = (difference / lastWeekStatistics) * 100;

            return percentageDifference;
        }

        /// <summary>
        /// Returns the first and last date of the selected month.
        /// It can be used for other weeks as well.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static (DateTime StartOfWeek, DateTime EndOfWeek) GetWeekBoundaries(DateTime date)
        {
            int diffStart = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = date.AddDays(-diffStart);

            int diffEnd = (7 + (DayOfWeek.Friday - date.DayOfWeek)) % 7;
            DateTime endOfWeek = date.AddDays(diffEnd);

            return (startOfWeek, endOfWeek);
        }
    }
}
