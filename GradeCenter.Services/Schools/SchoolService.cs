using GradeCenter.Data.Models;
using GradeCenter.Data;
using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services.Schools
{
    public class SchoolService : ISchoolService
    {
        private readonly GradeCenterContext _db;
        public SchoolService(GradeCenterContext gradeCenterContext)
        {
            _db = gradeCenterContext;
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
                .Where(school => school.IsActive)
                .FirstOrDefault(school => school.Id == id);

            return school;
        }

        /// <summary>
        /// Gets all existing school entries in the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<School> GetAllSchools()
        {
            var school = _db.Schools
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
            var currentSchool = GetSchoolById(updatedSchool.Id);

            if (currentSchool == null)
                return;

            currentSchool.Name = updatedSchool.Name;
            currentSchool.Address = updatedSchool.Address;

            await AddPrincipleToSchool(updatedSchool);

            if (updatedSchool.People.Any())
                currentSchool.People.Union(updatedSchool.People);

            await _db.SaveChangesAsync();
        }
        
        
        public async Task AddPrincipleToSchool(School? updatedSchool)
        {
            var currentPrinciple = _db.Users.FirstOrDefault(x => x.School.Id == updatedSchool.Id && x.UserRole == UserRoles.Principle);
            currentPrinciple.IsActive = false;

            var newPrinciple = updatedSchool.People.FirstOrDefault(x => x.UserRole == UserRoles.Principle);

            var currentSchool = _db.Schools.FirstOrDefault(x => x.Id == updatedSchool.Id);
            currentSchool.People.Remove(currentPrinciple);
            currentSchool.People.Add(newPrinciple); 

            await _db.SaveChangesAsync();
        }

        public async Task AddTeacherToSchool(School? updatedSchool)
        {
            var currentSchool = _db.Schools.FirstOrDefault(x => x.Id == updatedSchool.Id);

            var newTeachers = updatedSchool.People.Where(x => x.UserRole.HasValue && x.UserRole == UserRoles.Teacher && x.IsActive.HasValue && x.IsActive.Value).ToList();

            newTeachers.ForEach(t => currentSchool.People.Add(t));

            await _db.SaveChangesAsync();
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
