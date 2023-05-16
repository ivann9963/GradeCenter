using GradeCenter.Data.Models;

namespace GradeCenter.Services
{
    public interface ICurriculumService
    {
        void Create(List<Discipline> disciplines, SchoolClass schoolClass);
        void Update(List<Discipline> disciplines, SchoolClass schoolClass);
        void Delete(Discipline discipline);
    }
}
