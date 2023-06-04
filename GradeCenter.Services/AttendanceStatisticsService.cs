using GradeCenter.Data;
using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services.interfaces;

namespace GradeCenter.Services
{
    public class AttendanceStatisticsService : IAttendanceStatisticsService
    {
        private readonly GradeCenterContext _db;

        public AttendanceStatisticsService(GradeCenterContext db)
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

        public void CreateAttendanceStatistic(string? schoolId, string? schoolClassId, string? teacherId, string? disciplineName)
        {
            Statistic statistic = ExtractAverageRate(schoolId, schoolClassId, teacherId, disciplineName);

            
            statistic.CreatedOn = DateTime.UtcNow;
            statistic.StatisticType = StatisticTypes.Attendance;

            _db.Statistics.Add(statistic);
            _db.SaveChanges();
        }

        private Statistic ExtractAverageRate(string? schoolId, string? schoolClassId, string? teacherId, string? disciplineName)
        {
            Statistic statistic = new Statistic();

            statistic.AverageSchoolRate = SchoolAvgDisciplineAttendance(schoolId, disciplineName);
            statistic.School = _db.Schools.FirstOrDefault(x => x.Id == schoolId);

            statistic.AverageSchoolClassRate = ClassAvgDiscplineAttendance(schoolClassId, disciplineName);
            statistic.SchoolClass = _db.SchoolClasses.FirstOrDefault(x => x.Id == Guid.Parse(schoolClassId));

            statistic.AverageTeacherRate = TeacherAvgAttendances(teacherId);
            statistic.Teacher = _db.AspNetUsers.FirstOrDefault(x => x.Id == Guid.Parse(teacherId));


            return statistic;
        }

        private double TeacherAvgAttendances(string? teacherId)
        {
            if (teacherId == null)
                return -1;

            var disciplinesOwnedByTeacher = _db.Disciplines.Where(x => x.Teacher.Id == Guid.Parse(teacherId)).ToList()

            var studentsOfTeacherCount = disciplinesOwnedByTeacher.SelectMany(x => x.SchoolClass.Students).Distinct().Count();
            double possibleAttendances = studentsOfTeacherCount * disciplinesOwnedByTeacher.Count;
            double actualAttendances = disciplinesOwnedByTeacher.SelectMany(x => x.Attendances).Count();

            if (possibleAttendances == 0)
                return -1;

            double avgAttendance = actualAttendances / possibleAttendances;

            return avgAttendance;
        }


        private double ClassAvgDiscplineAttendance(string schoolClassId, string? disciplineName)
        {
            var schoolClass = _db.SchoolClasses.FirstOrDefault(c => c.Id == Guid.Parse(schoolClassId));

            if (schoolClass == null || schoolClass.Students.Count == 0 || schoolClass.Curriculum.Count == 0)
                return -1;

            double schoolClassStudentsCount = schoolClass.Students.Count;
            double curricullumDisciplineCount = schoolClass.Curriculum.Where(x => x.Name == disciplineName).Count();
            double disciplineActualAttendance = schoolClass.Curriculum.Where(d => d.Name == disciplineName).SelectMany(x => x.Attendances).Count();

            double possibleAttendances = schoolClassStudentsCount * curricullumDisciplineCount;

            if (possibleAttendances == 0)
                return -1;

            double avgAttendance = disciplineActualAttendance / possibleAttendances;

            return avgAttendance;
        }

        private double SchoolAvgDisciplineAttendance(string? schoolId, string? disciplineName)
        {
            var school = _db.Schools.FirstOrDefault(s => s.Id == schoolId);

            if (school == null || school.People.Count == 0 || school.SchoolClasses.Count == 0)
                return -1;

            double studentsInDisciplineCount = school.People.Where(r => r.UserRole == UserRoles.Student && r.SchoolClass.Curriculum.Any(c => c.Name == disciplineName)).Count();
            double classesHeldForDisciplineCount = school.SchoolClasses.SelectMany(x => x.Curriculum.Where(c => c.Name == disciplineName)).Count();
            double disciplineActualAttendances = school.SchoolClasses.SelectMany(g => g.Curriculum.Where(d => d.Name == disciplineName).SelectMany(g => g.Attendances)).Count();

            double totalPossibleAttendances = studentsInDisciplineCount * classesHeldForDisciplineCount;

            if (totalPossibleAttendances == 0)
                return -1;
            
            double avgAttendance = disciplineActualAttendances / totalPossibleAttendances;

            return avgAttendance;
        }
    }
}
