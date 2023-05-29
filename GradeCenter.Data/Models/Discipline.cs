using GradeCenter.Data.Models.Account;

namespace GradeCenter.Data.Models
{
    public class Discipline
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DayOfWeek OccuranceDay { get; set; } = DayOfWeek.Sunday;

        public TimeSpan OccuranceTime { get; set; } = new TimeSpan();

        public Guid SchoolClassId { get; set; }
        public SchoolClass SchoolClass { get; set; }

        public Guid TeacherId { get; set; }
        public AspNetUser Teacher { get; set; }

        public bool IsActive { get; set; }
        public List<Grade> Grades { get; set; }
    }
}
