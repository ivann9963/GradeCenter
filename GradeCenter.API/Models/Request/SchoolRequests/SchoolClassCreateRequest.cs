using GradeCenter.Data.Models.Account;

namespace GradeCenter.API.Models.Request.SchoolRequests
{
    public class SchoolClassCreateRequest
    {
        public int Year { get; set; }
        public string Department { get; set; }
        public string SchoolId { get; set; }
        public UserRequestModel Teacher { get; set; }
    }
}
