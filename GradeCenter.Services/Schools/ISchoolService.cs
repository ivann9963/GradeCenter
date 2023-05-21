﻿using GradeCenter.Data.Models;
using GradeCenter.Data.Models.Account;

namespace GradeCenter.Services.Schools
{
    public interface ISchoolService
    {

        School? GetSchoolByName(string name);
        School? GetSchoolById(string id);
        SchoolClass? GetSchoolClassById(string id);
        IEnumerable<School> GetAllSchools();
        Task Create(School newSchool);
        Task Update(School? updatedSchool);
        Task AddPrincipleToSchool(School? updatedSchool);
        Task AddTeachersToSchool(School? updatedSchool);
        Task AddStudentsToSchool(School? updatedSchool);
        Task Delete(string name);
        Task CreateClass(SchoolClass newSchoolClass);
        Task EnrollForClass(string classId, string studentId);
        Task WithdrawFromClass(string classId, string studentId);
    }
}
