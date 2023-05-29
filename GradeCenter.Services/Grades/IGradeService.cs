using GradeCenter.Data.Models;

namespace GradeCenter.Services.Grades
{
    public interface IGradeService
    {
        Task Create(Grade newGrade);
        Task Update(Grade? updatedGrade);
        Task Delete(string id);
        Grade GetGradeById(string id);
        IEnumerable<Grade> GetAllGrades();
    }
}
