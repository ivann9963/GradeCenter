using System;
using GradeCenter.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GradeCenter.Data;

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
        /// Takes an id as a paramater
        /// and returns the associated 
        /// School entity instance.
        /// </summary>
        /// <param name="id"></param>        
        /// <returns></returns>
        public School? GetSchoolById(int id)
        {
            var school = _db?.Schools?.FirstOrDefault(school => school.Id == id);

            return school;
        }

        /// <summary>
        /// Creates a new School entity instance
        /// in the database. Whilst ensuring there is a 
        /// relationship with a principal entity.
        /// </summary>
        /// <param name="newName"></param>
        /// <param name="newAddress"></param>
        /// <param name="principalId"></param>
        /// <param name="principalFirstName"></param>
        /// <param name="principalLastName"></param>
        /// <returns></returns> 
        public async Task Create(string newName, string newAddress, int principalId, string principalFirstName, string principalLastName)
        {
            var principal = _db?.Principals?.FirstOrDefault(principal => principal.Id == principalId);
            principal ??= EnsurePrincipalCreated(principalFirstName, principalLastName).GetAwaiter().GetResult();

            // TO DO: Decide whether to extract principal logic
            // in a distinguished service in order to follow seperation of concern principles.

            var school = new School
            {
                Name = newName,
                Address = newAddress,
                PrincipalId = principal.Id,
            };

            _db?.Schools?.AddAsync(school);

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing School 
        /// entity instance in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newName"></param>
        /// <param name="newAddress"></param>
        /// <returns></returns>
        public async Task Update(int id, string newName, string newAddress)
        {
            var school = GetSchoolById(id);

            if (school == null)
            {
                return;
            }
            
            school.Name = newName;
            school.Address = newAddress;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Delete an existing School entity instance
        /// in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task Delete(int id)
        {
            var school = GetSchoolById(id);

            if (school == null)
            {
                return;
            }

            school.IsDeleted = true;

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Ensures that a relational Principal entity
        /// is created.
        /// </summary>
        /// <param name="newFirstName"></param>
        /// <param name="newLastName"></param>
        /// <returns></returns>
        public async Task<Principal> EnsurePrincipalCreated(string newFirstName, string newLastName)
        {
            var principal = new Principal
            {
                FirstName = newFirstName,
                LastName = newLastName
            };

            _db?.Principals?.Add(principal);
            await _db.SaveChangesAsync();

            return principal;
        }
    }
}
