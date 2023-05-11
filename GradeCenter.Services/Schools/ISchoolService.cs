using GradeCenter.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeCenter.Services.Schools
{
    public interface ISchoolService
    {
        School? GetSchoolById(int id);
        Task Create(string newName, string newAddress, int principalId, string principalFirstName, string principalLastName);
        Task Update(int id, string newName, string newAddress);
        Task Delete(int id);
        Task<Principal> EnsurePrincipalCreated(string newFirstName, string newLastName);
    }
}
