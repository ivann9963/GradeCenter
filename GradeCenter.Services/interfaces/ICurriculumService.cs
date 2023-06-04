using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services.interfaces
{
    public interface ICurriculumService
    {
        void Create(List<Discipline> disciplines);
        void Update(List<Discipline> disciplines);
        void Delete(List<Discipline> disciplines);
        Discipline GetDisciplineByTeacherId(string teacherId);
        List<Discipline> GetClassesForDay(Guid schoolClassId, DayOfWeek day);
        List<Discipline> GetLoggedUserClasses(Guid userId);
        List<Discipline> GetCurricullumForSchoolClass(Guid schoolClassId);
    }
}
