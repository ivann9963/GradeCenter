using System.ComponentModel.DataAnnotations;

namespace GradeCenter.API.Models.Request.SchoolRequests
{
    public class SchoolCreateRequest
    {
        public string? Name { get; set; }

        public string? Address { get; set; }
    }
}
