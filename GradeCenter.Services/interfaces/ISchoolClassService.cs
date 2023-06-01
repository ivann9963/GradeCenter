using GradeCenter.Data.Models;

namespace GradeCenter.Services.interfaces
{
    public interface ISchoolClassService
    {
        Task CreateClass(SchoolClass newSchoolClass);
        Task EnrollForClass(string classId, string studentId);
        List<SchoolClass> GetAllClassess();
        List<SchoolClass> GetClassessInSchool(string schoolId);
        Task WithdrawFromClass(string studentId);
    }
}
