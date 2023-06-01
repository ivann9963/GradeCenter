using GradeCenter.Data.Models.Account;

namespace GradeCenter.Data.Models
{
    public class Statistic
    {
        public Guid Id { get; set; }

        public double Rate { get; set; }

        public SchoolClass? SchoolClass { get; set; }

        public School? School { get; set; }

        public AspNetUser? Teacher { get; set; }

        public Discipline? Discipline { get; set; } 

        public DateTime CreatedOn { get; set; }

    }
}
