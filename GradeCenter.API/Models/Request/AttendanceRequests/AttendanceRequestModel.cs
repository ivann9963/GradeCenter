namespace GradeCenter.API.Models.Request.AttendanceRequests
{
    public class AttendanceRequestModel
    {
        public string? Id { get; set; }
        public bool? HasAttended { get; set; }
        public DateTime Date { get; set; }
        public string StudentUsername { get; set; }
        public string DisciplineName { get; set; }


    }
}
