﻿using GradeCenter.Data;
using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services.Schools;
using Microsoft.EntityFrameworkCore;

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

        public List<SchoolClass> GetAllClassess()
        {
            var allClassess = _db.SchoolClasses
                .Include(s => s.School)
                .Include(t => t.HeadTeacher)
                .ToList();

            return allClassess;
        }

        /// <summary>
        /// Creates a new SchoolClass entity instance
        /// in the database.
        /// </summary>
        /// <param name="newSchoolClass"></param>
        /// <returns></returns>
        public async Task CreateClass(SchoolClass newSchoolClass)
        {
            var firstLastNames = $"{newSchoolClass.HeadTeacher.FirstName} {newSchoolClass.HeadTeacher.LastName}";

            var teacher = _accountService.GetUserByNames(firstLastNames); // TODO: make it take 2 argumants instead

            var school = _schoolService.GetSchoolByName(newSchoolClass.School.Name);

            if (teacher == null || school == null)
                return;

            newSchoolClass.HeadTeacher = teacher;
            newSchoolClass.School = school;

            await _db.SchoolClasses.AddAsync(newSchoolClass);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Enrolls a new Student entity in the School Classes collection.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task EnrollForClass(string className, string studentId)
        {
            var student = _accountService.GetUserById(studentId);
            var schoolClass = GetSchoolClassByName(className);

            if (student == null || schoolClass == null)
                return;

            if (IsStudentInClass(schoolClass, student))
                return;

            student.SchoolClass = schoolClass;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Withdraws an existing Student entity in the School Classes collection.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public async Task WithdrawFromClass(string studentId)
        {
            var student = _accountService.GetUserById(studentId);
            var schoolClass = student.SchoolClass;

            if (student == null || schoolClass == null)
                return;

            if (!IsStudentInClass(schoolClass, student))
                return;

            student.SchoolClass = null;

            await _db.SaveChangesAsync();
        }

        public SchoolClass GetSchoolClassByName(string schoolClassName)
        {
            int year = schoolClassName[0] - '0';
            string department = schoolClassName.Remove(0, 1);

            var schoolClass = _db.SchoolClasses.FirstOrDefault(sc => sc.Year == year && sc.Department == department);

            return schoolClass;
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
