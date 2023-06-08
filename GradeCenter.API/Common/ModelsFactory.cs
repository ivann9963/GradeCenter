using GradeCenter.API.Models.Request.AttendanceRequests;
using GradeCenter.API.Models.Request.CurricullumRequests;
using GradeCenter.API.Models.Request.CurricullumRequests;
using GradeCenter.API.Models.Request.GradeRequests;
using GradeCenter.API.Models.Request.SchoolRequests;
using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;
using System.Diagnostics;

namespace GradeCenter.API.Common
{
    public class ModelsFactory
    {
        public School ExtractSchool(SchoolCreateRequest requestModel)
        {
            var model = requestModel;
            List<AspNetUser> users = new List<AspNetUser>();

            if (requestModel is SchoolUpdateRequest updateRequest
                && updateRequest.Users != null && updateRequest.Users.Count >= 0)
            {
                model = updateRequest;
                users = updateRequest.Users.Select(x => new AspNetUser { Id = (Guid)x.UserId, UserRole = x.Role }).ToList();
            }

            return new School
            {
                Id = model is SchoolUpdateRequest updateModel ? updateModel.Id : null,
                Name = model.Name,
                Address = model.Address,
                People = users
            };
        }
        public SchoolClass ExtractSchoolClass(SchoolClassCreateRequest requestModel)
        {
            var model = requestModel;

            if (requestModel.TeacherNames == null)
                return null;

            return new SchoolClass
            {
                Id = Guid.Empty,
                Department = model.Department,
                HeadTeacher = new AspNetUser
                {
                    FirstName = requestModel.TeacherNames.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0],
                    LastName = requestModel.TeacherNames.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1],
                },
                School = new School
                {
                    Name = requestModel.SchoolName,
                },
                Year = model.Year
            };
        }

        public Discipline ExtractDiscipline(DisciplineDto dto)
        {
            var discipline = new Discipline()
            {
                Name = dto.Name,
                SchoolClassId = dto.SchoolClassId,
                TeacherId = dto.TeacherId,
                IsActive = true
            };

            if (dto.NewDay.HasValue)
                discipline.OccuranceDay = dto.NewDay.Value;

            if (dto.NewTime.HasValue)
                discipline.OccuranceTime = dto.NewTime.Value;

            return discipline;
        }

        public Grade ExtractGrade(GradeRequestModel requestModel)
        {
            if (requestModel.Number == null)
                return null;

            return new Grade
            {
                Id = requestModel.Id != null ? Guid.Parse(requestModel.Id) : Guid.Empty,
                Rate = requestModel.Number,
                Discipline = new Discipline
                {
                    Name = requestModel.DisciplineName
                },
                Student = new AspNetUser
                {
                    UserName = requestModel.StudentUsername
                }
            };
        }

        public List<Discipline> ExtractCurricullum(List<DisciplineDto> dtos)
        {
            var disciplines = new List<Discipline>();

            foreach (var dto in dtos)
            {
                var discipline = ExtractDiscipline(dto);
                disciplines.Add(discipline);
            }

            return disciplines;
        }

        public Attendance ExtractAttendance(AttendanceRequestModel requestModel)
        {
            return new Attendance
            {
                Id = requestModel.Id != null ? Guid.Parse(requestModel.Id) : Guid.Empty,
                HasAttended = requestModel.HasAttended,
                Date = requestModel.Date,
                Discipline = new Discipline
                {
                    Name = requestModel.DisciplineName
                },
                Student = new AspNetUser
                {
                    UserName = requestModel.StudentUsername
                }
            };
        }
    }
}
