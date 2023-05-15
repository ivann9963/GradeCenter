using GradeCenter.Data.Models.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GradeCenter.Data.Models
{
    public class School
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<AspNetUser> People { get; set; } = new HashSet<AspNetUser>();
    }
}
