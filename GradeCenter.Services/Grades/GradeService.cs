using GradeCenter.Data;
using GradeCenter.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GradeCenter.Services.Grades
{
    public class GradeService : IGradeService
    {
        private readonly GradeCenterContext _db;

        public GradeService(GradeCenterContext db)
        {
            _db = db;
        }
        public IEnumerable<Grade> GetAllGrades()
        {
            return _db.Grades
                .Include(discipline => discipline.Discipline)
                .Include(discipline => discipline.Discipline.Teacher)
                .Include(discipline => discipline.Discipline.SchoolClass)
                .Include(discipline => discipline.Discipline.SchoolClass.School)
                .Include(discipline => discipline.Student)
                .Where(grade => grade.IsActive)
                .ToList();
        }
        /// <summary>
        /// Creates a new Grade entity within the database.
        /// </summary>
        /// <param name="newGrade"></param>
        /// <returns></returns>
        public async Task Create(Grade newGrade)
        {
            var grade = new Grade
            {
                Rate = newGrade.Rate,
                Discipline = _db.Disciplines.FirstOrDefault(discipline => discipline.Name == newGrade.Discipline.Name),
                Student = _db.AspNetUsers.FirstOrDefault(student => student.UserName == newGrade.Student.UserName)
            };

            await _db.AddAsync(grade);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing Grade entity in the database.
        /// </summary>
        /// <param name="updateGrade"></param>
        /// <returns></returns>
        public async Task Update(Grade? updateGrade)
        {
            var grade = GetGradeById(updateGrade.Id.ToString());

            if (grade == null)
                return;

            grade.Rate = updateGrade.Rate;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Soft deletes an existing Grade entity in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Delete(string id)
        {
            var grade = GetGradeById(id);
            
            if (grade == null)
                return;

            grade.IsActive = false;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Gets an existing grade entity in the dabase
        /// by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Grade GetGradeById(string id)
        {
            return _db.Grades.FirstOrDefault(grade => grade.Id == Guid.Parse(id));
        }
    }
}
