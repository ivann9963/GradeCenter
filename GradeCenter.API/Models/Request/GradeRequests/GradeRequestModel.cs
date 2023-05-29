namespace GradeCenter.API.Models.Request.GradeRequests
{
    public class GradeRequestModel
    {
        public string? Id { get; set; }
        public int Number { get; set; }
        public string StudentUsername { get; set; }
        public string DisciplineName { get; set; }
    }
}
