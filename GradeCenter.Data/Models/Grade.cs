using GradeCenter.Data.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace GradeCenter.Data.Models
{
    public class Grade
    {
        [Key]
        public Guid Id { get; set; }
        [Range(2, 6)]
        public int Rate { get; set; }

        public bool IsActive { get; set; } = true;

        public Discipline Discipline { get; set; }

        public AspNetUser Student { get; set; }
    }
}
