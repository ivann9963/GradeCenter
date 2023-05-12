using System.ComponentModel.DataAnnotations;

namespace GradeCenter.API.Models.Request
{
    public class SchoolUpdateRequestModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }

        public ICollection<UserRequestModel>? Students { get; set; }
        public ICollection<UserRequestModel>? Teachers { get; set; }
        public UserRequestModel? Principal { get; set; }
    }
}
