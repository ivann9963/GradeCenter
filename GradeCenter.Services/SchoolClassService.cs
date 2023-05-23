using GradeCenter.Data;
using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services.Schools;

namespace GradeCenter.Services
{
    public class SchoolClassService : ISchoolClassService
    {
        private readonly GradeCenterContext _db;
        private readonly IAccountService _accountService;
        private readonly ISchoolService _schoolService;

        public SchoolClassService(GradeCenterContext db, IAccountService accountService, ISchoolService schoolService)
        {
            _db = db;
            _accountService = accountService;
            _schoolService = schoolService;
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
            var school = _schoolService.GetSchoolById(newSchoolClass.School.Id);

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
            var schoolClass = _schoolService.GetSchoolClassById(classId);

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
            var schoolClass = _schoolService.GetSchoolClassById(classId);

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
