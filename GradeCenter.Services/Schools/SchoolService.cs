using GradeCenter.Data.Models;
using GradeCenter.Data;
using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services.Schools
{
    public class SchoolService : ISchoolService
    {
        private readonly GradeCenterContext _db;
        private readonly IAccountService _accountService;

        public SchoolService(GradeCenterContext gradeCenterContext, IAccountService accountService)
        {
            _db = gradeCenterContext;
            this._accountService = accountService;
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

        public AspNetUser? GetTeacherById(string id)
        {
            var teacher = this._accountService.GetUserById(id);
            return teacher;
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
            var school = GetSchoolById(updatedSchool.Id);

            if (school == null)
                return;

            school.Name = updatedSchool.Name;
            school.Address = updatedSchool.Address;

            if (updatedSchool.People.Any(user => user.UserRole.Equals(UserRoles.Principle)))
                return;

            if (updatedSchool.People.Any())
                school.People.Union(updatedSchool.People);

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


        /// <summary>
        /// Creates a new SchoolClass entity instance
        /// in the database.
        /// </summary>
        /// <param name="newSchoolClass"></param>
        /// <returns></returns>
        public async Task CreateClass(SchoolClass newSchoolClass)
        {
            var teacher = GetTeacherById(newSchoolClass.HeadTeacher.Id.ToString());
            var school = GetSchoolById(newSchoolClass.School.Id);

            if (teacher == null)
                return;

            if (school == null)
                return;
            
            newSchoolClass.HeadTeacher = teacher;
            newSchoolClass.School = school;

            await _db.SchoolClasses.AddAsync(newSchoolClass);
            await _db.SaveChangesAsync();
        }
        public Task EnrollForClass(string id, AspNetUser student)
        {
            // TODO:...
            throw new NotImplementedException();
        }

        public Task WithdrawFromClass(string id, AspNetUser student)
        {
            // TODO:...
            throw new NotImplementedException();
        }
    }
}
