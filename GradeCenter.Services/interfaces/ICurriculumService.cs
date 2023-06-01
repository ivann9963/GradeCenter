using GradeCenter.Data.Models;

namespace GradeCenter.Services.interfaces
{
    public interface ICurriculumService
    {
        void Create(List<Discipline> disciplines);
        void Update(List<Discipline> disciplines);
        void Delete(List<Discipline> disciplines);
        List<Discipline> GetClassesForDay(Guid schoolClassId, DayOfWeek day);
        List<Discipline> GetLoggedUserClasses(Guid userId);
        List<Discipline> GetCurricullumForSchoolClass(Guid schoolClassId);
    }
}
