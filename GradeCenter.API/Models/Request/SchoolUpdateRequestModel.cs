using System.ComponentModel.DataAnnotations;

namespace GradeCenter.API.Models.Request
{
    public class SchoolUpdateRequestModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }

        public ICollection<UserRequestModel>? Users { get; set; }
    }
}
