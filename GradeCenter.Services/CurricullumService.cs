using GradeCenter.Data;
using GradeCenter.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GradeCenter.Services
{
    public class CurriculumService : ICurriculumService
    {
        private readonly GradeCenterContext _db;
        private static readonly Random Random = new Random();

        public CurriculumService(GradeCenterContext db)
        {
            _db = db;
        }

        public void Create(List<Discipline> disciplines)
        {
            var curriculum = GenerateCurriculum(disciplines);

            AddDisciplines(curriculum);
        }

        public void Update(List<Discipline> disciplines)
        {
            var updatedCurriculum = GenerateCurriculum(disciplines);

            UpdateDisciplines(updatedCurriculum);
        }

        public void Delete(List<Discipline> disciplines)
        {
            disciplines.ForEach(d => d.IsActive = false);
            
            UpdateDisciplines(disciplines);
        }

        private List<Discipline> GenerateCurriculum(List<Discipline> disciplines)
        {
            List<Discipline> currentCurriculum = new();

            if (!IsValid(disciplines))
                return currentCurriculum;

            foreach (var discipline in disciplines)
            {
                var teachersClasses = GetTeacherClasses(discipline);
                var freeSlot = GenerateAvailableDayAndTime(teachersClasses, discipline.SchoolClass, currentCurriculum);
                if (freeSlot.HasValue)
                {
                    discipline.OccuranceDay = freeSlot.Value.Key;
                    discipline.OccuranceTime = freeSlot.Value.Value;
                    currentCurriculum.Add(discipline);
                }
            }

            return currentCurriculum;
        }

        private KeyValuePair<DayOfWeek, TimeSpan>? GenerateAvailableDayAndTime(List<KeyValuePair<DayOfWeek, TimeSpan>> teachersClasses, SchoolClass schoolClass, List<Discipline> currentCurriculum)
        {
            // Random DayOfWeek
            DayOfWeek randomDay = (DayOfWeek)Random.Next(1, 5);

            // Random TimeSpan
            int startHour = 7;  // Start of the range (e.g., 8 AM)
            int endHour = 14;   // End of the range (e.g., 6 PM)
            int randomHour = Random.Next(startHour, endHour);
            int randomMinute = Random.Next(0, 60);

            TimeSpan randomTime = new(randomHour, randomMinute, 0);

            var schoolClassIsBusy = currentCurriculum.Any(d => d.OccuranceDay == randomDay && d.OccuranceTime == randomTime);
            var teacherIsBusy = teachersClasses.Any(c => c.Key == randomDay && c.Value == randomTime);

            if (schoolClassIsBusy || teacherIsBusy) 
                return GenerateAvailableDayAndTime(teachersClasses, schoolClass, currentCurriculum);

            return new KeyValuePair<DayOfWeek, TimeSpan>(randomDay, randomTime);
        }

        private bool IsValid(List<Discipline> disciplines)
        {
            var year = disciplines?.FirstOrDefault()?.SchoolClass.Year;
            return year != null && !disciplines.Any(d => d.SchoolClass.Year != year);
        }

        private List<KeyValuePair<DayOfWeek, TimeSpan>> GetTeacherClasses(Discipline discipline)
        {
            return _db.Disciplines
                .Where(t => t.Teacher == discipline.Teacher && t.SchoolClass.Year == discipline.SchoolClass.Year)
                .OrderBy(d => d.OccuranceDay)
                .ThenBy(t => t.OccuranceTime)
                .Select(x => new KeyValuePair<DayOfWeek, TimeSpan>(x.OccuranceDay, x.OccuranceTime))
                .ToList();
        }

        private void AddDisciplines(List<Discipline> disciplines)
        {
            ExecuteDbTransaction(() => _db.Disciplines.AddRange(disciplines));
        }

        private void UpdateDisciplines(List<Discipline> disciplines)
        {
            ExecuteDbTransaction(() => _db.Disciplines.UpdateRange(disciplines));
        }

        private void ExecuteDbTransaction(Action dbAction)
        {
            try
            {
                dbAction();
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
