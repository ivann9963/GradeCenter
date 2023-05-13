using GradeCenter.Data;
using Moq;
using Xunit;
using GradeCenter.Data.Models;
using GradeCenter.Services.Schools;
using Microsoft.EntityFrameworkCore;

namespace GradeCenter.Tests
{
    public class SchoolServiceTests
    {
        // Set up the necessary Mock objects and instantiate the SchoolService object
        private readonly Mock<GradeCenterContext> _dbMock;
        private readonly ISchoolService _schoolService;
        public SchoolServiceTests()
        {
            _dbMock = new Mock<GradeCenterContext>();
            _schoolService = new SchoolService(_dbMock.Object);
        }

        [Fact]
        public async Task Read_School_ReturnsEntries()
        {
            // Arrange
            var mockSettings = CreateDbSetMock();

            _dbMock.Setup(m => m.Schools).Returns(mockSettings.Object);

            // Act
            var result = _schoolService.GetAllSchools();

            // Assert
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public async Task Create_School_SavesChanges()
        {
            // Arrange
            // Set up the database sets.
            var schools = new List<School>();
            var mockSettings = CreateDbSetMock();
           
            // Set up the database mock to return 1 (success) when SaveChangesAsync is called
            // and ensure that the AddAsync method is called during run-time.
            _dbMock.Setup(m => m.Schools).Returns(mockSettings.Object);

            mockSettings.Setup(m => m.AddAsync(It.IsAny<School>(), default));
            _dbMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var school = new School
            {
                Address = "testAddress",
                Name = "testSchool"
            };

            await _schoolService.Create(school);

            // Assert
            // Verify that the SaveChangesAsync method on the SchoolService mock was called exactly once with the expected parameters.
            _dbMock.Verify(x => x.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task UpdateSchool_ShouldUpdateAddressAndName()
        {
            // Arrange
            var mockSettings = CreateDbSetMock();
            _dbMock.Setup(m => m.Schools).Returns(mockSettings.Object);

            // Act
            var updatedSchool = new School
            {
                Id = "e74d4ee1-fe78-4390-a971-5d7080a5dbf6",
                Address = "UpdatedAddress",
                Name = "UpdatedName"
            };

            await _schoolService.Update(updatedSchool);

            var school = _dbMock.Object.Schools.First();

            // Assert
            // that the School Address and Name fields are updated accordingly.
            Assert.Equal(school.Name, updatedSchool.Name);
            Assert.Equal(school.Address, updatedSchool.Address);

            // Verify that the SaveChangesAsync method on the SchoolService mock was called exactly once with the expected parameters.
            _dbMock.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteSchool_ShouldUpdateIsActive()
        {
            // Arrange
            var mockSettings = CreateDbSetMock();
            _dbMock.Setup(m => m.Schools).Returns(mockSettings.Object);

            //Act
            await _schoolService.Delete("testSchool");

            var school = _dbMock.Object.Schools.First();

            // Assert
            // that the School IsActive field is updated.
            // and verify that the SaveChangesAsync method on the SchoolService mock
            // was called exactly once with the expected parameters.
            Assert.Equal(false, school.IsActive);
            _dbMock.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }

        private static Mock<DbSet<School>> CreateDbSetMock()
        {
            var school = new School
            {
                Id = "e74d4ee1-fe78-4390-a971-5d7080a5dbf6",
                Address = "testAddress",
                Name = "testSchool"
            };

            var dbSetMock = new Mock<DbSet<School>>();
            var schools = new List<School>() { school }.AsQueryable();

            dbSetMock.As<IQueryable<School>>().Setup(m => m.Provider).Returns(schools.Provider);
            dbSetMock.As<IQueryable<School>>().Setup(m => m.Expression).Returns(schools.Expression);
            dbSetMock.As<IQueryable<School>>().Setup(m => m.ElementType).Returns(schools.ElementType);
            dbSetMock.As<IQueryable<School>>().Setup(m => m.GetEnumerator()).Returns(schools.GetEnumerator());

            return dbSetMock;
        }
    }
}
