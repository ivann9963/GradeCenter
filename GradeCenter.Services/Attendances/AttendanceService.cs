using GradeCenter.Data;
using GradeCenter.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeCenter.Services.Attendances
{
    public class AttendanceService : IAttendanceService
    {

        private readonly GradeCenterContext _db;

        public AttendanceService(GradeCenterContext db)
        {
            _db = db;
        }
        /// <summary>
        /// Creates a new Attendance entity
        /// </summary>
        /// <param name="newAttendance"></param>
        /// <returns></returns>
        public async Task Create(Attendance newAttendance)
        {
            var attendance = new Attendance
            {
                Date = DateTime.Now,
                Discipline = _db.Disciplines.FirstOrDefault(discipline =>
                    discipline.Name == newAttendance.Discipline.Name),
                Student = _db.AspNetUsers.FirstOrDefault(student =>
                    student.UserName == newAttendance.Student.UserName),
                HasAttended = newAttendance.HasAttended

            };

            await _db.AddAsync(attendance);
            await _db.SaveChangesAsync();
        }
        /// <summary>
        /// Update an existing attendance in the database
        /// </summary>
        /// <param name="updatedAttendance"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Update(Attendance? updatedAttendance)
        {
            var attendance = GetAttendanceById(updatedAttendance.Id.ToString());

            if (attendance == null)
                return;

            attendance.HasAttended = updatedAttendance.HasAttended;

            await _db.SaveChangesAsync();
        }
        /// <summary>
        /// Deletes an existing Attendance entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Delete(string id)
        {
            var attendance = GetAttendanceById(id);

            if (attendance == null)
                return;

            attendance.IsActive = false;

            await _db.SaveChangesAsync();
        }

        public IEnumerable<Attendance> GetAllAttendances()
        {
            return _db.Attendances
                .Include(discipline => discipline.Discipline)
                .Include(discipline => discipline.Student)
                .ToList();
        }

        public Attendance GetAttendanceById(string id)
        {
            return _db.Attendances.FirstOrDefault(attendance => attendance.Id == Guid.Parse(id));
        }


    }
}
