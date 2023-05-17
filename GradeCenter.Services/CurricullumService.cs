using GradeCenter.Data;
using GradeCenter.Data.Models;

namespace GradeCenter.Services
{
    public class CurriculumService : ICurriculumService
    {
        private readonly GradeCenterContext _db;

        public CurriculumService(GradeCenterContext db)
        {
            _db = db;
        }

        public void Create(List<Discipline> disciplines)
        {
            var curriculum = GenerateCurriculum(disciplines);

            _db.Disciplines.AddRange(curriculum);
        }

        public void Update(List<Discipline> disciplines)
        {
            var updatedCurriculum = GenerateCurriculum(disciplines);

            _db.Disciplines.UpdateRange(updatedCurriculum);
        }

        public void Delete(List<Discipline> disciplines)
        {
            disciplines.ForEach(d => d.IsActive = false);
            
            _db.Disciplines.UpdateRange(disciplines);
        }


        /// <summary>
        /// The GenerateCurriculum method creates a school curriculum. 
        /// It assigns each subject in the list a day and time, ensuring
        /// no clashes with the teacher's other classes or the class's other subjects. 
        /// It returns the list of subjects with their assigned slots, if available. 
        /// If the subject list is invalid, it returns an empty curriculum.
        /// </summary>
        /// <param name="disciplines"></param>
        /// <returns></returns>
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
            var allTimeSlots = GenerateAllTimeSlots();

            // Shuffle the list
            var randomTimeSlots = allTimeSlots.OrderBy(x => Guid.NewGuid()).ToList();
            
            // Find a free time slot
            var freeSlot = FindFreeTimeSlot(teachersClasses, currentCurriculum, randomTimeSlots);

            return freeSlot;
        }

        private static KeyValuePair<DayOfWeek, TimeSpan>? FindFreeTimeSlot(List<KeyValuePair<DayOfWeek, TimeSpan>> teachersClasses, List<Discipline> currentCurriculum, List<KeyValuePair<DayOfWeek, TimeSpan>> randomTimeSlots)
        {
            KeyValuePair<DayOfWeek, TimeSpan>? freeSlot = null;

            foreach (var slot in randomTimeSlots)
            {
                var day = slot.Key;
                var time = slot.Value;

                var schoolClassIsBusy = currentCurriculum.Any(d => d.OccuranceDay == day && d.OccuranceTime == time && d.IsActive);
                var teacherIsBusy = teachersClasses.Any(c => c.Key == day && c.Value == time);
                var maximumDisciplinesReached = currentCurriculum.Count(d => d.OccuranceDay == day) >= 7;

                if (!schoolClassIsBusy && !teacherIsBusy && !maximumDisciplinesReached)
                {
                    freeSlot = slot;
                    break;
                }
            }

            return freeSlot;
        }

        private List<KeyValuePair<DayOfWeek, TimeSpan>> GenerateAllTimeSlots()
        {
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

            return allTimeSlots;
        }

        private static bool IsValid(List<Discipline> disciplines)
        {
            var year = disciplines?.FirstOrDefault()?.SchoolClass.Year;
            return year != null && !disciplines.Any(d => d.SchoolClass.Year != year);
        }

        private List<KeyValuePair<DayOfWeek, TimeSpan>> GetTeacherClasses(Discipline discipline)
        {
            return _db.Disciplines
                .Where(t => t.Teacher == discipline.Teacher && t.SchoolClass.Year == discipline.SchoolClass.Year && discipline.IsActive)
                .OrderBy(d => d.OccuranceDay)
                .ThenBy(t => t.OccuranceTime)
                .Select(x => new KeyValuePair<DayOfWeek, TimeSpan>(x.OccuranceDay, x.OccuranceTime))
                .ToList();
        }
    }
}