using GradeCenter.Data;
using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services.interfaces;

namespace GradeCenter.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly GradeCenterContext _db;

        public StatisticsService(GradeCenterContext db)
        {
            _db = db;
        }

        public List<Statistic> GetStatistics(StatisticTypes statisticType, DateTime start, DateTime end)
        {
            return _db.Statistics
                .Where(s => s.CreatedOn >= start && s.CreatedOn <= end && s.StatisticType == statisticType)
                .ToList();
        }

        public List<Statistic> GetMonthly(StatisticTypes statisticType)
        {
            var (startOfMonth, endOfMonth) = GetMonthBoundaries(DateTime.UtcNow);
            return GetStatistics(statisticType, startOfMonth, endOfMonth);
        }

        public List<Statistic> GetWeekly(StatisticTypes statisticType)
        {
            var (startOfWeek, endOfWeek) = GetWeekBoundaries(DateTime.UtcNow);
            return GetStatistics(statisticType, startOfWeek, endOfWeek);
        }

        public List<Statistic> GetYearly(StatisticTypes statisticTypes)
        {
            var (startOfYear, endOfYear) = GetYearBoundaries(DateTime.UtcNow);
            return GetStatistics(statisticTypes, startOfYear, endOfYear);
        }

        public void CreateStatistic(string? schoolId, string? schoolClassId, string? teacherId, string? disciplineName, StatisticTypes statisticType)
        {
            var statistic = new Statistic
            {
                AverageRate = CalculateAverageRate(schoolId, schoolClassId, teacherId, disciplineName, statisticType),
                ComparedToLastWeek = CalculateComparedToLastWeek(statisticType),
                ComparedToLastMonth = CalculateComparedToLastMonth(statisticType),
                ComparedToLastYear = CalculateComparedToLastYear(statisticType),
                CreatedOn = DateTime.UtcNow,
                StatisticType = statisticType
            };

            statistic = SetSelectOption(statistic, schoolId, schoolClassId, teacherId);

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
        private Statistic SetSelectOption(Statistic statistic, string? schoolId, string? schoolClassId, string? teacherId)
        {
            if (schoolId != null && _db.Schools != null)
                statistic.School = _db.Schools.FirstOrDefault(x => x.Id == schoolId);
            else if (schoolClassId != null && _db.SchoolClasses != null)
                statistic.SchoolClass = _db.SchoolClasses.FirstOrDefault(x => x.Id == Guid.Parse(schoolClassId));
            else if (teacherId != null && _db.AspNetUsers != null)
                statistic.Teacher = _db.AspNetUsers.FirstOrDefault(x => x.Id == Guid.Parse(teacherId));

            return statistic;
        }

        private double CalculateAverageRate(string? schoolId, string? schoolClassId, string? teacherId, string? disciplineName, StatisticTypes statisticType)
        {
            double averageRate = 0;

            switch (statisticType)
            {
                case StatisticTypes.Grades:
                    if (schoolId != null)
                        averageRate = AvgSchoolGrade(schoolId, disciplineName);
                    else if (schoolClassId != null)
                        averageRate = AvgClassGrade(schoolClassId, disciplineName);
                    else if (teacherId != null)
                        averageRate = AvgTeacherGrade(teacherId);
                    break;

                case StatisticTypes.Attendance:
                    if (schoolId != null)
                        averageRate = SchoolAvgDisciplineAttendance(schoolId, disciplineName) ?? 0;
                    else if (schoolClassId != null)
                        averageRate = ClassAvgDiscplineAttendance(schoolClassId, disciplineName) ?? 0;
                    else if (teacherId != null)
                        averageRate = TeacherAvgAttendances(teacherId) ?? 0;
                    break;
            }

            return averageRate;
        }

        /// <summary>
        /// Calculates the average attendance rate for all 
        /// the disciplines owned by the selected teacher.
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        private double? TeacherAvgAttendances(string? teacherId)
        {
            if (teacherId == null)
                return null;

            var disciplinesOwnedByTeacher = _db.Disciplines.Where(x => x.Teacher.Id == Guid.Parse(teacherId)).ToList();

            var studentsOfTeacherCount = disciplinesOwnedByTeacher.SelectMany(x => x.SchoolClass.Students).Distinct().Count();
            double possibleAttendances = studentsOfTeacherCount * disciplinesOwnedByTeacher.Count;
            double actualAttendances = disciplinesOwnedByTeacher.SelectMany(x => x.Attendances).Count();

            if (possibleAttendances == 0)
                return null;

            double avgAttendance = actualAttendances / possibleAttendances;

            return avgAttendance;
        }


        /// <summary>
        /// Calculates the averate rate of the discipline 
        /// attendance for the whole school class.
        /// </summary>
        /// <param name="schoolClassId"></param>
        /// <param name="disciplineName"></param>
        /// <returns></returns>
        private double? ClassAvgDiscplineAttendance(string schoolClassId, string? disciplineName)
        {
            var schoolClass = _db.SchoolClasses.FirstOrDefault(c => c.Id == Guid.Parse(schoolClassId));

            if (schoolClass == null || schoolClass.Students.Count == 0 || schoolClass.Curriculum.Count == 0)
                return null;

            double schoolClassStudentsCount = schoolClass.Students.Count;
            double curricullumDisciplineCount = schoolClass.Curriculum.Where(x => x.Name == disciplineName).Count();
            double disciplineActualAttendance = schoolClass.Curriculum.Where(d => d.Name == disciplineName).SelectMany(x => x.Attendances).Count();

            double possibleAttendances = schoolClassStudentsCount * curricullumDisciplineCount;

            if (possibleAttendances == 0)
                return null;

            double avgAttendance = disciplineActualAttendance / possibleAttendances;

            return avgAttendance;
        }

        /// <summary>
        /// Calculates the average attenance for the 
        /// given discipline in a given school.
        /// </summary>
        /// <param name="schoolId"></param>
        /// <param name="disciplineName"></param>
        /// <returns></returns>
        private double? SchoolAvgDisciplineAttendance(string? schoolId, string? disciplineName)
        {
            var school = _db.Schools.FirstOrDefault(s => s.Id == schoolId);

            if (school == null || school.People.Count == 0 || school.SchoolClasses.Count == 0)
                return null;

            double studentsInDisciplineCount = school.People.Where(r => r.UserRole == UserRoles.Student && r.SchoolClass.Curriculum.Any(c => c.Name == disciplineName)).Count();
            double classesHeldForDisciplineCount = school.SchoolClasses.SelectMany(x => x.Curriculum.Where(c => c.Name == disciplineName)).Count();
            double disciplineActualAttendances = school.SchoolClasses.SelectMany(g => g.Curriculum.Where(d => d.Name == disciplineName).SelectMany(g => g.Attendances)).Count();

            double totalPossibleAttendances = studentsInDisciplineCount * classesHeldForDisciplineCount;

            if (totalPossibleAttendances == 0)
                return null;

            double avgAttendance = disciplineActualAttendances / totalPossibleAttendances;

            return avgAttendance;
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

            var schoolClassesGrades = _db.Grades
                .Where(g => g.Discipline.SchoolClass.School.Id == schoolId && g.Discipline.Name == disciplineName)
                .ToList();

            double avgGrade = 0;

            if(schoolClassesGrades.Count != 0)
                avgGrade = schoolClassesGrades.Average(x => x.Rate);

            return avgGrade;
        }

        /// <summary>
        /// Compares this year's Average of all AverageRates for Attendances
        /// to the last years.
        /// </summary>
        /// <returns></returns>
        private double CalculateComparedToLastYear(StatisticTypes statisticTypes)
        {
            var (startOfThisYear, endOfThisYear) = GetYearBoundaries(DateTime.Today);
            List<Statistic> currentYearStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfThisYear && x.CreatedOn <= endOfThisYear && x.StatisticType == statisticTypes).ToList();
            double currentYearStatisticsAvg = 0;

            if (currentYearStatistics.Count != 0)
                currentYearStatisticsAvg = currentYearStatistics.Average(x => x.AverageRate);

            var (startOfLastYear, endOfLastYear) = GetYearBoundaries(DateTime.Today.AddYears(-1));
            List<Statistic> lastYearStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfLastYear && x.CreatedOn <= endOfLastYear && x.StatisticType == statisticTypes).ToList();
            double lastYearStatisticsAvg = 0;

            if (lastYearStatistics.Count != 0)
                lastYearStatisticsAvg = lastYearStatistics.Average(x => x.AverageRate);

            double difference = currentYearStatisticsAvg - lastYearStatisticsAvg;
            double percentageDifference = Math.Round((difference / lastYearStatisticsAvg) * 100, 2);

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
        private double CalculateComparedToLastMonth(StatisticTypes statisticType)
        {
            var (startOfThisMonth, endOfThisMonth) = GetMonthBoundaries(DateTime.Today);
            List<Statistic> currentMonthStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfThisMonth && x.CreatedOn <= endOfThisMonth && x.StatisticType == statisticType).ToList();
            double currentMonthStatisticsAvg = 0;

            if (currentMonthStatistics.Count != 0)
                currentMonthStatisticsAvg = currentMonthStatistics.Average(x => x.AverageRate);

            var (startOfLastMonth, endOfLastMonth) = GetMonthBoundaries(DateTime.Today.AddMonths(-1));
            List<Statistic> lastMonthStatistics = _db.Statistics.Where(x => x.CreatedOn > startOfLastMonth && x.CreatedOn <= endOfLastMonth && x.StatisticType == statisticType).ToList();
            double lastMonthStatisticsAvg = 0;

            if(lastMonthStatistics.Count != 0)
                lastMonthStatisticsAvg = lastMonthStatistics.Average(x => x.AverageRate);

            double difference = currentMonthStatisticsAvg - lastMonthStatisticsAvg;
            double percentageDifference = Math.Round((difference / lastMonthStatisticsAvg) * 100, 2);

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
        private double CalculateComparedToLastWeek(StatisticTypes statisticType)
        {
            var (startOfLastWeek, endOfLastWeek) = GetWeekBoundaries(DateTime.Today.AddDays(-7));
            List<Statistic> lastWeekStatistics = _db.Statistics.Where(x => x.CreatedOn >= startOfLastWeek && x.CreatedOn <= endOfLastWeek && x.StatisticType == statisticType).ToList();

            double lastWeekStatisticsAvg = 0;

            if (lastWeekStatistics.Count != 0)
                lastWeekStatisticsAvg = lastWeekStatistics.Average(x => x.AverageRate);

            var (startOfWeek, endOfWeek) = GetWeekBoundaries(DateTime.Today);

            List<Statistic> currentWeekStatistics = _db.Statistics.Where(x => x.CreatedOn > startOfWeek && x.CreatedOn <= endOfWeek && x.StatisticType == statisticType).ToList();
            double currentWeekStatisticsAvg = 0;

            if(currentWeekStatistics.Count != 0)
                currentWeekStatisticsAvg = currentWeekStatistics.Average(x => x.AverageRate);

            double difference = currentWeekStatisticsAvg - lastWeekStatisticsAvg;
            double percentageDifference = Math.Round((difference / lastWeekStatisticsAvg) * 100, 2);

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
