using GradeCenter.Data.Models.Account;

namespace GradeCenter.Data.Models
{
    public class Statistic
    {
        public Guid Id { get; set; }

        public double AverageRate { get; set; }     // Average Grade or Attendance

        public double AverageSchoolRate { get; set; } // Average Grade or Attendance for the whole School

        public double AverageSchoolClassRate { get; set; } // Average Grade or Attendance for the SchoolClass

        public double AverageTeacherRate { get; set; } // Average Grade or Attendance for the Teacher's students

        public double ComparedToLastWeek { get; set; }  // Compares this weeks AverageRage to the last weeks's in % change

        public double ComparedToLastMonth { get; set; }  // Compares this weeks AverageRage to the last weeks's in % change

        public double ComparedToLastYear { get; set; }  // Compares this weeks AverageRage to the last weeks's in % change

        public SchoolClass? SchoolClass { get; set; }

        public School? School { get; set; }

        public AspNetUser? Teacher { get; set; }

        public string? DisciplineName { get; set; } 

        public DateTime CreatedOn { get; set; }

        public StatisticTypes StatisticType { get; set; }
    }

    public enum StatisticTypes
    {
        Grades = 0,
        Attendance = 1
    }
}
