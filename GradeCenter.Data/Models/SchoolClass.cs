using GradeCenter.Data.Models.Account;

namespace GradeCenter.Data.Models
{
    public class SchoolClass
    {
        public Guid Id { get; set; }

        public int Year { get; set; }

        public string Department { get; set; }

        public AspNetUser HeadTeacher { get; set; }

        public List<AspNetUser> Students { get; set; }

        public List<Discipline> Curriculum { get; set; }
    }
}
