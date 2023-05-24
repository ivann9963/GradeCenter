using GradeCenter.API.Models.Request.CurricullumRequests;
using GradeCenter.API.Models.Request.SchoolRequests;
using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;

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
                //Id = model is SchoolUpdateRequest updateModel ? updateModel.Id : null,
                Name = model.Name,
                Address = model.Address,
                People = users
            };
        }
        public SchoolClass ExtractSchoolClass(SchoolClassCreateRequest requestModel)
        {
            var model = requestModel;

            if (requestModel.Teacher == null && requestModel.Teacher.UserId == null)
                return null;

            return new SchoolClass
            {
                Id = Guid.Empty,
                Department = model.Department,
                HeadTeacher = new AspNetUser
                {
                    Id = (Guid)requestModel.Teacher.UserId,
                },
                School = new School
                {
                    Id = requestModel.SchoolId
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
    }
}
