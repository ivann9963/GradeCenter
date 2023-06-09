using GradeCenter.Data;
using GradeCenter.Data.Models.Account;
using Microsoft.EntityFrameworkCore;

namespace GradeCenter.Services
{
    public class UserRelationService : IUserRelationService
    {
        public readonly GradeCenterContext _db;
        public UserRelationService(GradeCenterContext db)
        {
            this._db = db;
        }
        /// <summary>
        /// Retrieves all user relation
        /// entries in the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserRelation> GetAll()
        {
            return _db.UserRelation
                .Include(ur => ur.Parent)
                .Include(ur => ur.Child)
                .ToList();
        }

        /// <summary>
        /// Retrieves all user relation
        /// entries based on a given student name.
        /// </summary>
        /// <param name="studentName"></param>
        /// <returns></returns>
        public IEnumerable<UserRelation> GetByStudentName(string studentName)
        {
            return _db.UserRelation
                .Where(userRelation => userRelation.Child.UserName == studentName)    
                .Include(ur => ur.Parent)
                .Include(ur => ur.Child)
                .ToList();
        }

        /// <summary>
        /// Retrieves all user relation
        /// entries based on a given parent name.
        /// </summary>
        /// <param name="parentName"></param>
        /// <returns></returns>
        public IEnumerable<UserRelation> GetByParentName(string parentName)
        {
            return _db.UserRelation
                .Where(userRelation => userRelation.Parent.UserName == parentName)
                .Include(ur => ur.Parent)
                .Include(ur => ur.Child)
                .ToList();
        }
    }
}
