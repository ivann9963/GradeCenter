using GradeCenter.Data.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradeCenter.Data.Models
{
    public class School
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Number { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public bool IsActive { get; set; } = true;
        [ForeignKey("Principal")]
        public string? PrincipalId { get; set; }
        public virtual User? Principal { get; set; }
    }
}
