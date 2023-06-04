using GradeCenter.Data;
using GradeCenter.Data.Models;
using GradeCenter.Services.interfaces;

namespace GradeCenter.Services
{
    public class CurriculumService : ICurriculumService
    {
        private readonly GradeCenterContext _db;

        public CurriculumService(GradeCenterContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Creates a new curriculum by adding the provided disciplines to the database.
        /// </summary>
        /// <param name="disciplines"></param>
        public void Create(List<Discipline> disciplines)
        {
            var curriculum = GenerateCurriculum(disciplines);

            _db.Disciplines.AddRange(curriculum);
            _db.SaveChanges();
        }

        /// <summary>
        /// Updates existing disciplines in the database with the provided ones.
        /// </summary>
        /// <param name="updatedDisciplines"></param>
        public void Update(List<Discipline> updatedDisciplines)
        {
            var currentDisciplines = new List<Discipline>();

            foreach (var discipline in updatedDisciplines)
            {
                var currentDiscipline = _db.Disciplines.FirstOrDefault(d => d.Name == discipline.Name
                    && d.TeacherId == discipline.TeacherId && d.SchoolClassId == discipline.SchoolClassId);

                currentDiscipline.OccuranceDay = discipline.OccuranceDay;
                currentDiscipline.OccuranceTime = discipline.OccuranceTime;

                currentDisciplines.Add(currentDiscipline);

                _db.SaveChanges();
            }

            var schoolClassId = updatedDisciplines.FirstOrDefault().SchoolClassId;

            var currentSchoolClassCurricullum = GetCurricullumForSchoolClass(schoolClassId)
                .Where(x => !currentDisciplines.Any(d => d.Id == x.Id))
                .ToList();

            var updatedCurriculum = GenerateCurriculum(currentSchoolClassCurricullum);

            _db.SaveChanges();
        }

        /// <summary>
        /// Marks provided disciplines as inactive in the database.
        /// </summary>
        /// <param name="disciplines"></param>
        public async void Delete(List<Discipline> disciplines)
        {
            foreach (var discipline in disciplines)
            {
                var currentDiscipline = _db.Disciplines.FirstOrDefault(d => d.Name == discipline.Name
                    && d.TeacherId == discipline.TeacherId && d.SchoolClassId == discipline.SchoolClassId);

                currentDiscipline.IsActive = false;

            }

            _db.SaveChanges();
        }

        /// <summary>
        /// Fetches classes for a specific day for a given schoolClass from the database.
        /// </summary>
        /// <param name="schoolClassId"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public List<Discipline> GetClassesForDay(Guid schoolClassId, DayOfWeek day)
        {
            var disciplines = _db.Disciplines.Where(x => x.SchoolClass.Id == schoolClassId && x.OccuranceDay == day && x.IsActive).ToList();

            return disciplines;
        }

        /// <summary>
        /// Fetches active classes for a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Discipline> GetLoggedUserClasses(Guid userId)
        {
            var discipline = _db.Disciplines.Where(x => x.SchoolClass.Students.Any(s => s.Id == userId) && x.IsActive).ToList();

            return discipline;
        }

        /// <summary>
        /// Assigns each subject in the list a day and time, 
        /// ensuring no clashes with the teacher's or the schoolClass's other classes. 
        /// It returns the list of disciplines with their assigned timeslot, if available. 
        /// If the subject list is invalid, it returns an empty curriculum.
        /// </summary>
        /// <param name="disciplines"></param>
        /// <returns></returns>
        private List<Discipline> GenerateCurriculum(List<Discipline> disciplines)
        {
            List<Discipline> currentCurriculum = new();

            disciplines.ForEach((discipline) =>
            {
                discipline.Teacher ??= _db?.AspNetUsers?.FirstOrDefault(x => x.Id == discipline.TeacherId);
                discipline.SchoolClass ??= _db?.SchoolClasses?.FirstOrDefault(x => x.Id == discipline.SchoolClassId);
            });

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

        /// <summary>
        /// Finds a free slot for a class considering the teacher's schedule and the existing curriculum.
        /// </summary>
        /// <param name="teachersClasses"></param>
        /// <param name="schoolClass"></param>
        /// <param name="currentCurriculum"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Determines a free slot that doesn't clash with the current curriculum or the teacher's classes.
        /// </summary>
        /// <param name="teachersClasses"></param>
        /// <param name="currentCurriculum"></param>
        /// <param name="randomTimeSlots"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Generates a list of all possible time slots within a week, excluding weekends.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if the provided list of disciplines belongs to the same academic year.
        /// </summary>
        /// <param name="disciplines"></param>
        /// <returns></returns>
        private static bool IsValid(List<Discipline> disciplines)
        {
            var year = disciplines?.FirstOrDefault()?.SchoolClass.Year;
            return year != null && !disciplines.Any(d => d.SchoolClass.Year != year);
        }

        /// <summary>
        /// Fetches a teacher's active classes from the database.
        /// </summary>
        /// <param name="discipline"></param>
        /// <returns></returns>
        private List<KeyValuePair<DayOfWeek, TimeSpan>> GetTeacherClasses(Discipline discipline)
        {
            return _db.Disciplines
                .Where(t => t.Teacher == discipline.Teacher && t.SchoolClass.Year == discipline.SchoolClass.Year && discipline.IsActive)
                .OrderBy(d => d.OccuranceDay)
                .ThenBy(t => t.OccuranceTime)
                .Select(x => new KeyValuePair<DayOfWeek, TimeSpan>(x.OccuranceDay, x.OccuranceTime))
                .ToList();
        }

        public List<Discipline> GetCurricullumForSchoolClass(Guid schoolClassId)
        {
            var schoolClassCurricullum = _db.Disciplines
                .Where(d => d.SchoolClassId == schoolClassId)
                .ToList();

            return schoolClassCurricullum;
        }

        /// <summary>
        /// Get teacher entity for respective discipline
        /// </summary>
        /// <param name="disciplineName"></param>
        /// <returns></returns>
        public Discipline GetDisciplineByTeacherId(string teacherId)
        {
            var discipline = _db.Disciplines
                   .FirstOrDefault(f => f.TeacherId == Guid.Parse(teacherId));

            return discipline;
        }
    }
}