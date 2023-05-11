namespace GradeCenter.API.Models.Request
{
    public class SchoolCreateRequestModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PrincipalFirstName { get; set; }
        public string PrincipalLastName { get; set; }
        public int PrincipalId { get; set; }
    }
}
