using GradeCenter.Data.Models;

namespace GradeCenter.Services
{
    public interface ISchoolClassService
    {
        Task CreateClass(SchoolClass newSchoolClass);
        Task EnrollForClass(string classId, string studentId);
        List<SchoolClass> GetAllClassess();
        Task WithdrawFromClass(string studentId);
    }
}
