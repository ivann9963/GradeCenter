using System;
using GradeCenter.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GradeCenter.Data;
using GradeCenter.Data.Models.Account;
using System.Collections;

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
        /// Gets all existing school entries in the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<School> Read()
        {
            return _db.Schools.Where(school => school.IsActive)
                .ToList();
        }

        /// <summary>
        /// Takes an number as a paramater
        /// and returns the associated 
        /// School entity instance.
        /// </summary>
        /// <param name="number"></param>        
        /// <returns></returns>
        public School? GetSchoolByNumber(int number)
        {
            var school = _db?.Schools?.FirstOrDefault(school => school.Number == number);

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
                Number = newSchool.Number,
                Address = newSchool.Address
            };

            await _db.Schools.AddAsync(school);

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing School 
        /// entity instance in the database.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="updatedSchool"></param>
        /// <param name="students"></param>
        /// <param name="teachers"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public async Task Update(int number, School? updatedSchool, IEnumerable<User>? updatedStudents, IEnumerable<User>? updatedTeachers, User? updatedPrincipal)
        {
            var school = GetSchoolByNumber(number);

            if (school == null) return;
            
            school.Name = updatedSchool.Name;
            school.Address = updatedSchool.Address;


            if (updatedStudents != null && school.Principal != null) school.Principal.Students.Union(updatedStudents);

            if (updatedTeachers != null && school.Principal != null) school.Principal.Teachers.Union(updatedTeachers);

            if (updatedPrincipal != null && school.Principal != null) school.Principal = updatedPrincipal;


            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete an existing School entity instance
        /// in the database.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task Delete(int number)
        {
            var school = GetSchoolByNumber(number);

            if (school == null)
            {
                return;
            }

            school.IsActive = false;

            await _db.SaveChangesAsync();
        }

    }
}
