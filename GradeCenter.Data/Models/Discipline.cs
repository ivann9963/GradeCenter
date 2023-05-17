using GradeCenter.Data.Models.Account;

namespace GradeCenter.Data.Models
{
    public class Discipline
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DayOfWeek OccuranceDay { get; set; }

        public TimeSpan OccuranceTime { get; set; }

        public SchoolClass SchoolClass { get; set; }

        public AspNetUser Teacher { get; set; }

        public bool IsActive { get; set; }
    }
}
