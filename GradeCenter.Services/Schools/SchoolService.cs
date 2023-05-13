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
            => _db?.Schools
                   .Where(school => school.IsActive)
                   .FirstOrDefault(school => school.Name.ToLower() == name.ToLower());

        /// <summary>
        /// Takes an id as a paramater
        /// and returns the associated
        /// School entity instance.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public School? GetSchoolById(string id)
            => _db?.Schools
                   .Where(school => school.IsActive)
                   .FirstOrDefault(school => school.Id.ToString() == id);

        /// <summary>
        /// Gets all existing school entries in the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<School> GetAllSchools()
            => _db.Schools
                 .Where(school => school.IsActive)
                 .ToList();

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
            var school = GetSchoolById(updatedSchool.Id.ToString());

            if (school == null)
                return;

            school.Name = updatedSchool.Name;
            school.Address = updatedSchool.Address;

            if (updatedSchool.Users.Any(user => user.UserRole.Equals(UserRoles.Principle)))
                return;

            if (updatedSchool.Users.Any())
                school.Users.Union(updatedSchool.Users);

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
