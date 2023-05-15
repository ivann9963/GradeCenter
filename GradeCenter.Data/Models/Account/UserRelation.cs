namespace GradeCenter.Data.Models.Account
{
    public class UserRelation
    {
        public Guid ParentId { get; set; }
        public User Parent { get; set; }

        public Guid ChildId { get; set; }
        public User Child { get; set; }
    }
}
