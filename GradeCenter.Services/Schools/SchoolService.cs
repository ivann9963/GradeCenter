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
            await AddTeachersToSchool(updatedSchool);
            await AddStudentsToSchool(updatedSchool);

            if (updatedSchool.People.Any())
                currentSchool.People.Union(updatedSchool.People);

            await _db.SaveChangesAsync();
        }

        public async Task AddPrincipleToSchool(School? updatedSchool)
        {
            if (!updatedSchool.People.Any(x => x.UserRole == UserRoles.Principle))
                return;

            var newPrinciple = updatedSchool.People.FirstOrDefault(x => x.UserRole == UserRoles.Principle);
            var currentSchool = _db.Schools.FirstOrDefault(x => x.Id == updatedSchool.Id);

            var currentPrincipleExist = _db.AspNetUsers.Any(u => u.School.Id == updatedSchool.Id && u.UserRole == UserRoles.Principle);

            if (currentPrincipleExist)
            {
                var currentPrinciple = _db.AspNetUsers.FirstOrDefault(x => x.School.Id == updatedSchool.Id && x.UserRole == UserRoles.Principle);
                currentPrinciple.IsActive = false;
                currentSchool.People.Remove(currentPrinciple);
            }
            currentSchool.People.Add(newPrinciple);

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

            var currentSchool = _db.Schools.FirstOrDefault(x => x.Id == updatedSchool.Id);

            var newStudents = updatedSchool.People.Where(x => x.UserRole.HasValue && x.UserRole == UserRoles.Student && x.IsActive.HasValue && x.IsActive.Value).ToList();

            newStudents.ForEach(s => currentSchool.People.Add(s));

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
            var teacher = _accountService.GetUserById(newSchoolClass.HeadTeacher.Id.ToString());
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

        /// <summary>
        /// Enrolls a new Student entity in the School Classes collection.
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task EnrollForClass(string classId, string studentId)
        {
            var student = _accountService.GetUserById(studentId);
            var schoolClass = GetSchoolClassById(classId);

            if (student == null)
                return;

            if (schoolClass == null)
                return;

            if (IsStudentInClass(schoolClass, student))
                return;

            schoolClass.Students.Add(student);

            await this._db.SaveChangesAsync();
        }
        /// <summary>
        /// Withdraws an existing Student entity in the School Classes collection.
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task WithdrawFromClass(string classId, string studentId)
        {
            var student = _accountService.GetUserById(studentId);
            var schoolClass = GetSchoolClassById(classId);

            if (student == null)
                return;

            if (schoolClass == null)
                return;

            if (IsStudentInClass(schoolClass, student))
                return;

            schoolClass.Students.Remove(student);

            await this._db.SaveChangesAsync();
        }

        /// <summary>
        /// Asserts whether there is already an existing Student 
        /// within a School Class.
        /// </summary>
        /// <param name="schoolClass"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool IsStudentInClass(SchoolClass schoolClass, AspNetUser user)
        {
            var inClass = schoolClass.Students.Any(student => student.Id == user.Id);

            return inClass;
        }
    }
}
