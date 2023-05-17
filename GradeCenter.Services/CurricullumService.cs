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
            // Generate all possible time slots
            var allTimeSlots = new List<KeyValuePair<DayOfWeek, TimeSpan>>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday) continue; // Skip weekends

                for (int hour = 7; hour < 14; hour++)
                {
                    for (int minute = 0; minute < 60; minute += 10) // Assuming classes start every 10 minutes
                    {
                        allTimeSlots.Add(new KeyValuePair<DayOfWeek, TimeSpan>(day, new TimeSpan(hour, minute, 0)));
                    }
                }
            }

            // Shuffle the list
            var randomTimeSlots = allTimeSlots.OrderBy(x => Random.Next()).ToList();
            KeyValuePair<DayOfWeek, TimeSpan>? freeSlot = null;

            // Find a free time slot
            foreach (var slot in randomTimeSlots)
            {
                var day = slot.Key;
                var time = slot.Value;

                var schoolClassIsBusy = currentCurriculum.Any(d => d.OccuranceDay == day && d.OccuranceTime == time);
                var teacherIsBusy = teachersClasses.Any(c => c.Key == day && c.Value == time);
                var disciplinesForDay = currentCurriculum.Count(d => d.OccuranceDay == day);
                var maximumDisciplinesReached = disciplinesForDay >= 7;

                if (!schoolClassIsBusy && !teacherIsBusy && !maximumDisciplinesReached)
                {
                    freeSlot = slot;
                    break;
                }
            }

            return freeSlot;
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
                throw;
            }
        }
    }
}
