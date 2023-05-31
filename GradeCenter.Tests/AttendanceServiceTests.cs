using Moq;
using Xunit;
using GradeCenter.Data.Models.Account;
using GradeCenter.Data;
using GradeCenter.Data.Models;
using Microsoft.EntityFrameworkCore;
using GradeCenter.Services.Attendances;

namespace GradeCenter.Tests
{
    public class AttendanceServiceTests
    {
        // Set up the necessary Mock objects and instantiate the SchoolService object
        private readonly Mock<GradeCenterContext> _dbMock;
        private readonly IAttendanceService _attendanceService;

        public AttendanceServiceTests()
        {
            _dbMock = new Mock<GradeCenterContext>();

            // Set up the database mock to return the DbSet mock for Users
            var users = new List<AspNetUser> {
                new AspNetUser
                {
                    FirstName = "Peter",
                    LastName = "Takell",
                    UserRole = UserRoles.Student
                },
                new AspNetUser
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    UserRole = UserRoles.Teacher
                },
            }.AsQueryable();


            var usersMock = new Mock<DbSet<AspNetUser>>();
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());


            _dbMock.Setup(x => x.AspNetUsers).Returns(usersMock.Object);

            var school = new School
            {
                Id = "aae2d3d8-602f-46e1-b982-3097361f5207",
                Address = "testAddress",
                Name = "testSchool",
                People = users.ToList()
            };

            foreach (var user in school.People)
            {
                // Set the school reference for each user
                user.School = school;
            }

            var schoolMock = new Mock<DbSet<School>>();
            var schools = new List<School>() { school }.AsQueryable();


            schoolMock.As<IQueryable<School>>().Setup(m => m.Provider).Returns(schools.Provider);
            schoolMock.As<IQueryable<School>>().Setup(m => m.Expression).Returns(schools.Expression);
            schoolMock.As<IQueryable<School>>().Setup(m => m.ElementType).Returns(schools.ElementType);
            schoolMock.As<IQueryable<School>>().Setup(m => m.GetEnumerator()).Returns(schools.GetEnumerator());


            _dbMock.Setup(x => x.Schools).Returns(schoolMock.Object);

            // Set up the database mock to return the DbSet mock for Discipline
            var disciplineMock = new Mock<DbSet<Discipline>>();

            var discipline = new Discipline
            {
                Id = Guid.Parse("aae2d3d8-602f-46e1-b982-3097361f5207"),
                IsActive = true,
                OccuranceDay = DayOfWeek.Monday,
                Name = "Math",
                OccuranceTime = TimeSpan.MinValue
            };

            var disciplines = new List<Discipline>() { discipline }.AsQueryable();

            disciplineMock.As<IQueryable<Discipline>>().Setup(m => m.Provider).Returns(disciplines.Provider);
            disciplineMock.As<IQueryable<Discipline>>().Setup(m => m.Expression).Returns(disciplines.Expression);
            disciplineMock.As<IQueryable<Discipline>>().Setup(m => m.ElementType).Returns(disciplines.ElementType);
            disciplineMock.As<IQueryable<Discipline>>().Setup(m => m.GetEnumerator()).Returns(disciplines.GetEnumerator());

            _dbMock.Setup(x => x.Disciplines).Returns(disciplineMock.Object);

            // Set up the database mock to return the DbSet mock for Attendance
            var attendanceMock = new Mock<DbSet<Attendance>>();

            var Attendance = new Attendance
            {
                Id = Guid.Parse("3144e6f5-9f45-452a-8914-8e7decbdf271"),
                Date = DateTime.Now,
                IsActive = true,
                HasAttended = false,
                Discipline = _dbMock.Object.Disciplines.ToList().FirstOrDefault(),
                Student = _dbMock.Object.AspNetUsers.ToList().FirstOrDefault()
            };

            var attendances = new List<Attendance>() { Attendance }.AsQueryable();

            attendanceMock.As<IQueryable<Attendance>>().Setup(m => m.Provider).Returns(attendances.Provider);
            attendanceMock.As<IQueryable<Attendance>>().Setup(m => m.Expression).Returns(attendances.Expression);
            attendanceMock.As<IQueryable<Attendance>>().Setup(m => m.ElementType).Returns(attendances.ElementType);
            attendanceMock.As<IQueryable<Attendance>>().Setup(m => m.GetEnumerator()).Returns(attendances.GetEnumerator());

            _dbMock.Setup(x => x.Attendances).Returns(attendanceMock.Object);

            _attendanceService = new AttendanceService(_dbMock.Object);
        }

        [Fact]
        public async Task Read_Attendances_ReturnsEntries()
        {
            // Act
            var result = _attendanceService.GetAllAttendances();

            // Assert
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public async Task Create_Attendance_SavesChanges()
        {
            // Act
            var discipline = _dbMock.Object.Disciplines.FirstOrDefault();
            var student = _dbMock.Object.AspNetUsers.FirstOrDefault();

            var Attendance = new Attendance
            {
                Discipline = discipline,
                Student = student,
                HasAttended = true,
            };

            await _attendanceService.Create(Attendance);

            // Assert
            // Verify that the SaveChangesAsync method on the Attendance mock was called exactly once with the expected parameters.
            _dbMock.Verify(x => x.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task UpdateAttendance_ShouldUpdateHasAttendedProperty()
        {
            // Act
            var updateAttendance = new Attendance
            {
                Id = Guid.Parse("3144e6f5-9f45-452a-8914-8e7decbdf271"),
                IsActive = true,
                HasAttended = true,
                Discipline = _dbMock.Object.Disciplines.ToList().FirstOrDefault(),
                Student = _dbMock.Object.AspNetUsers.ToList().FirstOrDefault()
            };

            await _attendanceService.Update(updateAttendance);

            var Attendance = _dbMock.Object.Attendances.First();

            // Assert
            // that the Attendance HasAttended bool property is updated accordingly.
            Assert.Equal(Attendance.HasAttended, updateAttendance.HasAttended);

            _dbMock.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAttendance_ShouldUpdateIsActive()
        {
            //Act
            await _attendanceService.Delete("3144e6f5-9f45-452a-8914-8e7decbdf271");

            var Attendance = _dbMock.Object.Attendances.First();

            // Assert
            // that the School IsActive field is updated.
            // and verify that the SaveChangesAsync method on the SchoolService mock
            // was called exactly once with the expected parameters.
            Assert.Equal(false, Attendance.IsActive);
            _dbMock.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }
    }
}