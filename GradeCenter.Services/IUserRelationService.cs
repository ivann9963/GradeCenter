using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services
{
    public interface IUserRelationService
    {
        IEnumerable<UserRelation> GetAll();
        IEnumerable<UserRelation> GetByStudentName(string studentName);
        IEnumerable<UserRelation> GetByParentName(string parentName);
    }
}
