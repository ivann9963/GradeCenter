using GradeCenter.Data.Models.Account;
using Microsoft.AspNetCore.Identity;

namespace GradeCenter.Data.Models
{
    public class Discipline
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Occurance { get; set; }

        public SchoolClass SchoolClass { get; set; }

        public AspNetUser Teacher { get; set; }

        public bool IsActive { get; set; }
    }
}
