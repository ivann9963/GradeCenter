namespace GradeCenter.API.Models.Request
{
    public class UserRequestModel
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
