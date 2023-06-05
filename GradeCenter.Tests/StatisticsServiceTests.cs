namespace GradeCenter.Tests
{
    using Moq;
    using Xunit;
    using GradeCenter.Data;
    using GradeCenter.Data.Models;
    using GradeCenter.Services;
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using GradeCenter.Data.Models.Account;

    public class StatisticsServiceTests
    {
        private Mock<DbSet<Statistic>> _mockStatistics;
        private Mock<GradeCenterContext> _mockContext;
        private StatisticsService _service;

        public StatisticsServiceTests()
        {
            _mockContext = new Mock<GradeCenterContext>();

            var schoolDummy = new School
            {
                Id = "testSchoolId",
                Name = "Test School",
                Address = "Test Address",
            };

            var dummySchoolClasses = new List<SchoolClass>
            {
                new SchoolClass
                {
                    Id = Guid.NewGuid(),
                    Year = 2023,
                    Department = "Test Department",
                    School = schoolDummy
                }
            }.AsQueryable();

            schoolDummy.SchoolClasses = dummySchoolClasses.ToList();

            var discipline = new Discipline
            {
                Id = Guid.Parse("e74d4ee1-fe78-4390-a971-5d7080a5dbf6"),
                IsActive = true,
                OccuranceDay = DayOfWeek.Monday,
                Name = "Math",
                OccuranceTime = TimeSpan.MinValue,
                SchoolClass = dummySchoolClasses.First(),
            };

            dummySchoolClasses.First().Curriculum = new List<Discipline> { discipline };

            var disciplines = new List<Discipline>() { discipline }.AsQueryable();

            var disciplineMock = new Mock<DbSet<Discipline>>();
            disciplineMock.As<IQueryable<Discipline>>().Setup(m => m.Provider).Returns(disciplines.Provider);
            disciplineMock.As<IQueryable<Discipline>>().Setup(m => m.Expression).Returns(disciplines.Expression);
            disciplineMock.As<IQueryable<Discipline>>().Setup(m => m.ElementType).Returns(disciplines.ElementType);
            disciplineMock.As<IQueryable<Discipline>>().Setup(m => m.GetEnumerator()).Returns(disciplines.GetEnumerator());

            _mockContext.Setup(x => x.Disciplines).Returns(disciplineMock.Object);

            var dummySchools = new List<School>
            {
                schoolDummy,
            }.AsQueryable();

            var mockSchools = new Mock<DbSet<School>>();
            mockSchools.As<IQueryable<School>>().Setup(m => m.Provider).Returns(dummySchools.Provider);
            mockSchools.As<IQueryable<School>>().Setup(m => m.Expression).Returns(dummySchools.Expression);
            mockSchools.As<IQueryable<School>>().Setup(m => m.ElementType).Returns(dummySchools.ElementType);
            mockSchools.As<IQueryable<School>>().Setup(m => m.GetEnumerator()).Returns(dummySchools.GetEnumerator());

            _mockContext.Setup(m => m.Schools).Returns(mockSchools.Object);

            var mockSchoolClasses = new Mock<DbSet<SchoolClass>>();
            mockSchoolClasses.As<IQueryable<SchoolClass>>().Setup(m => m.Provider).Returns(dummySchoolClasses.Provider);
            mockSchoolClasses.As<IQueryable<SchoolClass>>().Setup(m => m.Expression).Returns(dummySchoolClasses.Expression);
            mockSchoolClasses.As<IQueryable<SchoolClass>>().Setup(m => m.ElementType).Returns(dummySchoolClasses.ElementType);
            mockSchoolClasses.As<IQueryable<SchoolClass>>().Setup(m => m.GetEnumerator()).Returns(dummySchoolClasses.GetEnumerator());
            _mockContext.Setup(m => m.SchoolClasses).Returns(mockSchoolClasses.Object);

            var dummyAspNetUsers = new List<AspNetUser>
            {
                new AspNetUser { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User" }
            }.AsQueryable();

            var mockAspNetUsers = new Mock<DbSet<AspNetUser>>();
            mockAspNetUsers.As<IQueryable<AspNetUser>>().Setup(m => m.Provider).Returns(dummyAspNetUsers.Provider);
            mockAspNetUsers.As<IQueryable<AspNetUser>>().Setup(m => m.Expression).Returns(dummyAspNetUsers.Expression);
            mockAspNetUsers.As<IQueryable<AspNetUser>>().Setup(m => m.ElementType).Returns(dummyAspNetUsers.ElementType);
            mockAspNetUsers.As<IQueryable<AspNetUser>>().Setup(m => m.GetEnumerator()).Returns(dummyAspNetUsers.GetEnumerator());

            _mockContext.Setup(m => m.AspNetUsers).Returns(mockAspNetUsers.Object);

            var dummyStatistics = new List<Statistic>() { }.AsQueryable();

            _mockStatistics = new Mock<DbSet<Statistic>>();
            _mockStatistics.As<IQueryable<Statistic>>().Setup(m => m.Provider).Returns(dummyStatistics.Provider);
            _mockStatistics.As<IQueryable<Statistic>>().Setup(m => m.Expression).Returns(dummyStatistics.Expression);
            _mockStatistics.As<IQueryable<Statistic>>().Setup(m => m.ElementType).Returns(dummyStatistics.ElementType);
            _mockStatistics.As<IQueryable<Statistic>>().Setup(m => m.GetEnumerator()).Returns(dummyStatistics.GetEnumerator());
            _mockContext.Setup(m => m.Statistics).Returns(_mockStatistics.Object);

            // Create a list of mock grades
            var dummyGrades = new List<Grade>
            {
                new Grade { Id = Guid.NewGuid(), IsActive = true, Rate = 85, Discipline = discipline },
            }.AsQueryable();

            // Create a mock DbSet of Grades
            var mockGrades = new Mock<DbSet<Grade>>();
            mockGrades.As<IQueryable<Grade>>().Setup(m => m.Provider).Returns(dummyGrades.Provider);
            mockGrades.As<IQueryable<Grade>>().Setup(m => m.Expression).Returns(dummyGrades.Expression);
            mockGrades.As<IQueryable<Grade>>().Setup(m => m.ElementType).Returns(dummyGrades.ElementType);
            mockGrades.As<IQueryable<Grade>>().Setup(m => m.GetEnumerator()).Returns(dummyGrades.GetEnumerator());

            // Assign the mock grades to the context
            _mockContext.Setup(m => m.Grades).Returns(mockGrades.Object);

            // Finally, assign the grades to your discipline
            discipline.Grades = dummyGrades.ToList();


            _service = new StatisticsService(_mockContext.Object);
        }

        [Fact]
        public void CreateSchoolStatistic_AddsNewStatisticToDatabase()
        {
            // Arrange
            var schoolId = "testSchoolId";
            var disciplineName = "Math";
            var statisticType = StatisticTypes.Grades;

            // Act
            _service.CreateStatistic(schoolId, null, null, disciplineName, statisticType);

            // Assert
            _mockStatistics.Verify(m => m.Add(It.IsAny<Statistic>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
    }

}
