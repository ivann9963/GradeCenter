using GradeCenter.Data.Models;

namespace GradeCenter.Services
{
    public interface ICurriculumService
    {
        void Create(List<Discipline> disciplines);
        void Update(List<Discipline> disciplines);
        void Delete(List<Discipline> disciplines);
    }
}
