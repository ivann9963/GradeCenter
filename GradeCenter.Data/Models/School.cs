using GradeCenter.Data.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradeCenter.Data.Models
{
    public class School: BaseModel<int>
    {
        public School()
        {
            this.Students = new HashSet<Student>();
            this.Parents = new HashSet<Parent>();
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [ForeignKey("Principal")]
        public int PrincipalId { get; set; }
        public Principal Principal { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Parent> Parents { get; set; }
    }
}
