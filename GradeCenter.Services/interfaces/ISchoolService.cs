using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services.interfaces
{
    public interface ISchoolService
    {

        School? GetSchoolByName(string name);
        School? GetSchoolById(string id);
        SchoolClass? GetSchoolClassById(string id);
        IEnumerable<School> GetAllSchools();
        Task Create(School newSchool);
        Task Update(School? updatedSchool);
        Task AddPrincipleToSchool(School? updatedSchool);
        Task AddTeachersToSchool(School? updatedSchool);
        Task AddStudentsToSchool(School? updatedSchool);
        Task Delete(string name);
        List<AspNetUser> GetPeopleInSchool(string schoolId);
    }
}
