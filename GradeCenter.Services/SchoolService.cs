using GradeCenter.Data.Models;
using GradeCenter.Data;
using GradeCenter.Data.Models.Account;
using Microsoft.EntityFrameworkCore;
using GradeCenter.Services.interfaces;

namespace GradeCenter.Services
{
    public class SchoolService : ISchoolService
    {
        private readonly GradeCenterContext _db;
        private readonly IAccountService _accountService;

        public SchoolService(GradeCenterContext gradeCenterContext, IAccountService accountService)
        {
            _db = gradeCenterContext;
            _accountService = accountService;
        }

        /// <summary>
        /// Takes a name as a paramater
        /// and returns the associated 
        /// School entity instance.
        /// </summary>
        /// <param name="name"></param>        
        /// <returns></returns>
        public School? GetSchoolByName(string name)
        {
            var school = _db?.Schools
                   .Where(school => school.IsActive)
                   .FirstOrDefault(school => school.Name.ToLower() == name.ToLower());

            return school;
        }

        /// <summary>
        /// Takes an id as a paramater
        /// and returns the associated
        /// School entity instance.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public School? GetSchoolById(string id)
        {
            var school = _db?.Schools
                .Include(p => p.People)
                .Include(sc => sc.SchoolClasses)
                .ThenInclude(ht => ht.HeadTeacher)
                .Where(school => school.IsActive)
                .FirstOrDefault(school => school.Id == id);

            return school;
        }

        /// <summary>
        /// Gets all people in the selected school.
        /// </summary>
        /// <param name="schoolId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<AspNetUser> GetPeopleInSchool(string schoolId)
        {
            var people = _db.AspNetUsers
                .Include(s => s.School)
                .Include(c => c.SchoolClass)
                .Where(s => s.School.Id == schoolId)
                .ToList();

            return people;
        }

        public SchoolClass? GetSchoolClassById(string id)
        {
            var schoolClass = _db?.SchoolClasses
                 .FirstOrDefault(schoolClass => schoolClass.Id == Guid.Parse(id));

            return schoolClass;
        }
        /// <summary>
        /// Gets all existing school entries in the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<School> GetAllSchools()
        {
            var school = _db.Schools
                .Include(X => X.People)
                .Where(school => school.IsActive)
                .ToList();

            return school;
        }

        /// <summary>
        /// Creates a new School entity instance
        /// in the database. Whilst ensuring there is a 
        /// relationship with a principal entity.
        /// </summary>
        /// <param name="newSchool"></param>
        /// <returns></returns> 
        public async Task Create(School newSchool)
        {
            var school = new School
            {
                Name = newSchool.Name,
                Address = newSchool.Address
            };

            await _db.Schools.AddAsync(school);

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing School 
        /// entity instance in the database.
        /// </summary>
        /// <param name="schoolName"></param>
        /// <param name="updatedSchool"></param>
        /// <param name="students"></param>
        /// <param name="teachers"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public async Task Update(School? updatedSchool)
        {
            var currentSchool = GetSchoolByName(updatedSchool.Name);

            if (currentSchool == null)
                return;

            if (updatedSchool.Address != null)
                currentSchool.Address = updatedSchool.Address;

            await AddPrincipleToSchool(updatedSchool);
            await AddTeachersToSchool(updatedSchool);
            await AddStudentsToSchool(updatedSchool);

            await _db.SaveChangesAsync();
        }

        public async Task AddPrincipleToSchool(School? updatedSchool)
        {
            if (!updatedSchool.People.Any(x => x.UserRole == UserRoles.Principle))
                return;

            var newPrinciple = updatedSchool.People.FirstOrDefault(x => x.UserRole == UserRoles.Principle);

            var newPrincipleFromDB = _db.AspNetUsers.FirstOrDefault(x => x.Id == newPrinciple.Id);
            var currentSchool = _db.Schools.FirstOrDefault(x => x.Name.ToLower() == updatedSchool.Name.ToLower());

            var currentPrincipleExist = _db.AspNetUsers.Any(u => u.School.Id == updatedSchool.Id && u.UserRole == UserRoles.Principle);

            if (currentPrincipleExist)
            {
                var currentPrinciple = _db.AspNetUsers.FirstOrDefault(x => x.School.Id == updatedSchool.Id && x.UserRole == UserRoles.Principle);
                currentPrinciple.School = null;
            }

            var school = _db.Schools.FirstOrDefault(x => x.Id == updatedSchool.Id);
            newPrincipleFromDB.SchoolId = school.Id;
            newPrincipleFromDB.School = school;
            newPrincipleFromDB.UserRole = UserRoles.Principle;

            await _db.SaveChangesAsync();
        }

        public async Task AddTeachersToSchool(School? updatedSchool)
        {
            if (!updatedSchool.People.Any(x => x.UserRole == UserRoles.Teacher))
                return;

            var currentSchool = _db.Schools.FirstOrDefault(x => x.Id == updatedSchool.Id);
            var newTeachers = updatedSchool.People.Where(x => x.UserRole.HasValue && x.UserRole == UserRoles.Teacher && x.IsActive.HasValue && x.IsActive.Value).ToList();
            newTeachers.ForEach(t => currentSchool.People.Add(t));

            await _db.SaveChangesAsync();
        }

        public async Task AddStudentsToSchool(School? updatedSchool)
        {
            if (!updatedSchool.People.Any(x => x.UserRole == UserRoles.Student))
                return;

            var currentSchool = _db.Schools.FirstOrDefault(x => x.Name == updatedSchool.Name);

            var newStudents = updatedSchool.People.Where(x => x.UserRole.HasValue && x.UserRole == UserRoles.Student).ToList();

            foreach (var student in newStudents)
            {
                var currentSudents = _db.Users.FirstOrDefault(u => u.Id == student.Id);
                currentSudents.SchoolId = currentSchool.Id;
                currentSudents.School = currentSchool;

                _db.SaveChanges();
            }
        }

        /// <summary>
        /// Delete an existing School entity instance
        /// in the database.
        /// </summary>
        /// <param name="schoolName"></param>
        /// <returns></returns>
        public async Task Delete(string schoolName)
        {
            var school = GetSchoolByName(schoolName);

            if (school == null)
                return;

            school.IsActive = false;

            await _db.SaveChangesAsync();
        }
    }
}
