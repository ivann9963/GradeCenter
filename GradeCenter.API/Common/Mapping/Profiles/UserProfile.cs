using AutoMapper;
using GradeCenter.API.Models.Request;
using GradeCenter.Data.Models.Account;

namespace GradeCenter.API.Common.Mapping.Profiles
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<UserRequestModel, User>();
        }
    }
}
