using Moq;
using Xunit;
using GradeCenter.Data.Models.Account;
using GradeCenter.Data;
using GradeCenter.Data.Models;
using Microsoft.EntityFrameworkCore;
using GradeCenter.Services.Grades;

namespace GradeCenter.Tests
{
    public class GradeServiceTests
    {
        // Set up the necessary Mock objects and instantiate the SchoolService object
        private readonly Mock<GradeCenterContext> _dbMock;
        private readonly IGradeService _gradeService;

        public GradeServiceTests()
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
                Id = "e74d4ee1-fe78-4390-a971-5d7080a5dbf6",
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
                Id = Guid.Parse("e74d4ee1-fe78-4390-a971-5d7080a5dbf6"),
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

            // Set up the database mock to return the DbSet mock for Grade
            var gradeMock = new Mock<DbSet<Grade>>();

            var grade = new Grade
            {
                Id = Guid.Parse("5b4d2c62-44e9-44d0-a208-e4192a92c34d"),
                IsActive = true,
                Rate = 6,
                Discipline = _dbMock.Object.Disciplines.ToList().FirstOrDefault(),
                Student = _dbMock.Object.AspNetUsers.ToList().FirstOrDefault()
            };

            var grades = new List<Grade>() { grade }.AsQueryable();

            gradeMock.As<IQueryable<Grade>>().Setup(m => m.Provider).Returns(grades.Provider);
            gradeMock.As<IQueryable<Grade>>().Setup(m => m.Expression).Returns(grades.Expression);
            gradeMock.As<IQueryable<Grade>>().Setup(m => m.ElementType).Returns(grades.ElementType);
            gradeMock.As<IQueryable<Grade>>().Setup(m => m.GetEnumerator()).Returns(grades.GetEnumerator());

            _dbMock.Setup(x => x.Grades).Returns(gradeMock.Object);

            _gradeService = new GradeService(_dbMock.Object);
        }

        [Fact]
        public async Task GetAllGrades_ReturnsCorrectData()
        {
            // Act
            var result = _gradeService.GetAllGrades();

            // Assert
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public async Task Create_Grade_SavesChanges()
        {
            // Act
            var discipline = _dbMock.Object.Disciplines.FirstOrDefault();
            var student = _dbMock.Object.AspNetUsers.FirstOrDefault();

            var grade = new Grade
            {
                Discipline = discipline,
                Student = student,
                Rate = 5,
            };

            await _gradeService.Create(grade);

            // Assert
            // Verify that the SaveChangesAsync method on the GradeService mock was called exactly once with the expected parameters.
            _dbMock.Verify(x => x.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task UpdateGrade_ShouldUpdateNumber()
        {
            // Act
            var updateGrade = new Grade
            {
                Id = Guid.Parse("5b4d2c62-44e9-44d0-a208-e4192a92c34d"),
                IsActive = true,
                Rate = 5,
                Discipline = _dbMock.Object.Disciplines.ToList().FirstOrDefault(),
                Student = _dbMock.Object.AspNetUsers.ToList().FirstOrDefault()
            };

            await _gradeService.Update(updateGrade);

            var grade = _dbMock.Object.Grades.First();

            // Assert
            // that the Grade Number is updated accordingly.
            Assert.Equal(grade.Rate, updateGrade.Rate);

            // Verify that the SaveChangesAsync method on the GradeService mock was called exactly once with the expected parameters.
            _dbMock.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteGrade_ShouldUpdateIsActive()
        {
            //Act
            await _gradeService.Delete("5b4d2c62-44e9-44d0-a208-e4192a92c34d");

            var grade = _dbMock.Object.Grades.First();

            // Assert
            // that the School IsActive field is updated.
            // and verify that the SaveChangesAsync method on the SchoolService mock
            // was called exactly once with the expected parameters.
            Assert.Equal(false, grade.IsActive);
            _dbMock.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }
    }
}
