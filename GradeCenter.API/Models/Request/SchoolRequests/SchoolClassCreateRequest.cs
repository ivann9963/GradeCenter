using GradeCenter.Data.Models.Account;

namespace GradeCenter.API.Models.Request.SchoolRequests
{
    public class SchoolClassCreateRequest
    {
        public int Year { get; set; }
        public string Department { get; set; }
        public string SchoolName { get; set; }
        public string TeacherNames { get; set; }
    }
}
