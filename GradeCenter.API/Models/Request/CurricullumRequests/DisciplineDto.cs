using GradeCenter.Data.Models.Account;
using GradeCenter.Data.Models;

namespace GradeCenter.API.Models.Request.CurricullumRequests
{
    public class DisciplineDto
    {
        public string Name { get; set; }

        public Guid SchoolClassId { get; set; }

        public Guid TeacherId { get; set; }
    }
}
