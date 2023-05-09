using GradeCenter.Data.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeCenter.Data.Models
{
    public class Student : BaseModel<int>
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [ForeignKey("School")]
        public int SchoolId { get; set; }
        public School School { get; set; }
    }
}
