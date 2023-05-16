using GradeCenter.Data;
using GradeCenter.Data.Models;

namespace GradeCenter.Services
{
    public class CurricullumService : ICurriculumService
    {
        private readonly GradeCenterContext _db;

        public CurricullumService(GradeCenterContext db)
        {
            _db = db;
        }

        public void Create(List<Discipline> disciplines, SchoolClass schoolClass)
        {
            schoolClass = CreateSchedule(disciplines, schoolClass);

            _db.SchoolClasses.Add(schoolClass);
            _db.SaveChanges();
        }
        public void Delete(Discipline discipline)
        {
            discipline.IsActive = false;

            _db.Disciplines.Update(discipline);
            _db.SaveChanges();
        }

        public void Update(List<Discipline> disciplines, SchoolClass schoolClass)
        {
            schoolClass.Curriculum = disciplines;

            _db.SchoolClasses.Update(schoolClass);
            _db.SaveChanges();
        }

        private SchoolClass CreateSchedule(List<Discipline> disciplines, SchoolClass schoolClass)
        {
            var allClassesInYear = _db.SchoolClasses.Where(sc => sc.Year == schoolClass.Year);

            foreach (var yearClass in allClassesInYear)
            {
                if (yearClass.Curriculum.Count == 0)
                    continue;

                //TODO:...
            }

            return null;
        }
    }
}
