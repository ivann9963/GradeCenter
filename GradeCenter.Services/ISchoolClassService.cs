using GradeCenter.Data.Models;

namespace GradeCenter.Services
{
    public interface ISchoolClassService
    {
        Task CreateClass(SchoolClass newSchoolClass);
        Task EnrollForClass(string classId, string studentId);
        Task WithdrawFromClass(string classId, string studentId);
    }
}
