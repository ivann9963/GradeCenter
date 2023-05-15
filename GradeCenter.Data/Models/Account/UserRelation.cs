namespace GradeCenter.Data.Models.Account
{
    public class UserRelation
    {
        public Guid ParentId { get; set; }
        public AspNetUser Parent { get; set; }

        public Guid ChildId { get; set; }
        public AspNetUser Child { get; set; }
    }
}
