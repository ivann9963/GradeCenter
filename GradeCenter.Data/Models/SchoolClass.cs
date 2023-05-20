using GradeCenter.Data.Models.Account;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradeCenter.Data.Models
{
    public class SchoolClass
    {
        public Guid Id { get; set; }

        public int Year { get; set; }

        public string Department { get; set; }

        public AspNetUser HeadTeacher { get; set; }
        public School School { get; set; }

        public List<AspNetUser> Students { get; set; } = new List<AspNetUser>();

        public List<Discipline> Curriculum { get; set; } = new List<Discipline>();
    }
}
