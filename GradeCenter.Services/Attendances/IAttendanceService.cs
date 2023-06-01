using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GradeCenter.Data.Models;

namespace GradeCenter.Services.Attendances
{
    public interface IAttendanceService
    {
        Task Create(Attendance attendance);
        Task Update(Attendance? updatedAttendance);
        Task Delete(string id);
        
        Attendance GetAttendanceById(string id);
        IEnumerable<Attendance> GetAllAttendances();

    }
}
