using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services.Schools
{
    public interface ISchoolService
    {
        School? GetSchoolByNumber(int number);
        IEnumerable<School> Read();
        Task Create(School newSchool);
        Task Update(int number, School? updatedSchool, IEnumerable<User>? updatedStudents, IEnumerable<User>? updatedTeachers, User? newPrincipal);
        Task Delete(int number);
    }
}
