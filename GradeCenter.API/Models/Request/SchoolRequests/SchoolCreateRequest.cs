using System.ComponentModel.DataAnnotations;

namespace GradeCenter.API.Models.Request.SchoolRequests
{
    public class SchoolCreateRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
