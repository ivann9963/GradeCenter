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
                users = updateRequest.Users.Select(x => new AspNetUser { Id = x.UserId }).ToList();
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

            return new SchoolClass
            {
                Id = Guid.Empty,
                Department = model.Department,
                HeadTeacher = new AspNetUser
                {
                    Id = requestModel.Teacher.UserId,
                },
                Year = model.Year
            };
        }
    }
}
