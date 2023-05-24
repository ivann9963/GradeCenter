using System.ComponentModel.DataAnnotations;

namespace GradeCenter.API.Models.Request.SchoolRequests
{
    public class SchoolUpdateRequest : SchoolCreateRequest
    {
        public string? Id { get; set; }

        public ICollection<UserRequestModel>? Users { get; set; }
    }
}
