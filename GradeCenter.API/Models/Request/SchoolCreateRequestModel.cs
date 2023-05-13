using System.ComponentModel.DataAnnotations;

namespace GradeCenter.API.Models.Request
{
    public class SchoolCreateRequestModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
