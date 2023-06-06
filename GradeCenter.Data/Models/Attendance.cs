using GradeCenter.Data.Models.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeCenter.Data.Models
{
    public class Attendance
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }

        public Discipline Discipline { get; set; }

        public bool IsActive { get; set; } = true;

        public AspNetUser Student { get; set; }

        public bool? HasAttended { get; set; }
    }
}
