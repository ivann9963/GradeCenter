using AutoMapper;
using GradeCenter.Data.Models;
using GradeCenter.API.Models.Request;

namespace GradeCenter.API.Common.Mapping.Profiles
{
    public class SchoolProfile: Profile
    {
        public SchoolProfile()
        {
            CreateMap<SchoolCreateRequestModel, School>();
            CreateMap<SchoolUpdateRequestModel, School>();
        }
    }
}
