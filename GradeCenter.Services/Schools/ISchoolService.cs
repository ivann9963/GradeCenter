using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services.Schools
{
    public interface ISchoolService
    {

        School? GetSchoolByName(string name);
        School? GetSchoolById(string id);
        AspNetUser? GetTeacherById(string id);

        IEnumerable<School> GetAllSchools();
        Task Create(School newSchool);
        Task Update(School? updatedSchool);
        Task Delete(string name);
        Task CreateClass(SchoolClass newSchoolClass);
        Task EnrollForClass(string id, AspNetUser student);
        Task WithdrawFromClass(string id, AspNetUser student);
    }
}
