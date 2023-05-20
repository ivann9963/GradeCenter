using GradeCenter.Data;
using Moq;
using Xunit;
using GradeCenter.Data.Models;
using GradeCenter.Services.Schools;
using Microsoft.EntityFrameworkCore;
using GradeCenter.Services;
using GradeCenter.Data.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace GradeCenter.Tests
{
    public class SchoolServiceTests
    {
        // Set up the necessary Mock objects and instantiate the SchoolService object
        private readonly Mock<GradeCenterContext> _dbMock;
        private readonly Mock<UserManager<AspNetUser>> _userManagerMock;
        private readonly Mock<SignInManager<AspNetUser>> _signInManagerMock;
        private readonly ISchoolService _schoolService;
        private readonly IAccountService _accountService;
        public SchoolServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<AspNetUser>>();
            _userManagerMock = new Mock<UserManager<AspNetUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AspNetUser>>(_userManagerMock.Object, new Mock<IHttpContextAccessor>().Object, new Mock<IUserClaimsPrincipalFactory<AspNetUser>>().Object, null, null, null);
            _dbMock = new Mock<GradeCenterContext>();
           
            _accountService = new AccountService(_userManagerMock.Object, _dbMock.Object, _signInManagerMock.Object);
            _schoolService = new SchoolService(_dbMock.Object, _accountService);
        }

        [Fact]
        public async Task Read_School_ReturnsEntries()
        {
            // Arrange
            var mockSettings = CreateDbSetSchoolMock();

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
            var mockSettings = CreateDbSetSchoolMock();
           
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
            var mockSettings = CreateDbSetSchoolMock();
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
            var mockSettings = CreateDbSetSchoolMock();
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

        [Fact]
        public async Task Create_SchoolClass_Saves_Changes()
        {
            // Arrange
            
            // Set up the database sets and mocked School Class for create operations.
            var schoolClass = new SchoolClass
            {
                Id = Guid.NewGuid(),
                Department = "testDepartmentNew",
                HeadTeacher = new AspNetUser
                {
                    Id = Guid.Parse("3cfa002b-7175-4900-9060-55a5c795ab50"),
                    UserName = "testTeacherNew",
                    FirstName = "testFirstNameNew",
                    LastName = "testLastNameNew",
                    SchoolId = "e74d4ee1-fe78-4390-a971-5d7080a5dbf6"
                },
                School = new School
                {
                    Id = "e74d4ee1-fe78-4390-a971-5d7080a5dbf6",
                    Address = "SomeAdressNew",
                    Name = "testSchoolNew"
                },
                Year = 2023
            };

            // Set up the database sets and mocked School Class for create operations.
            var mockSettings = CreateDbSetSchoolClassMock();
            _dbMock.Setup(m => m.SchoolClasses).Returns(CreateDbSetSchoolClassMock().Object);
            _dbMock.Setup(m => m.Schools).Returns(CreateDbSetSchoolMock().Object);
            _dbMock.Setup(m => m.AspNetUsers).Returns(CreateDbSetUsers().Object);

            // Set up the database mock to return 1 (success) when SaveChangesAsync is called
            // and ensure that the AddAsync method is called during run-time.
            mockSettings.Setup(m => m.AddAsync(It.IsAny<SchoolClass>(), default));
            _dbMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            await _schoolService.CreateClass(schoolClass);

            // Assert
            // Verify that the SaveChangesAsync method on the SchoolService mock was called exactly once with the expected parameters.
            _dbMock.Verify(x => x.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task Enroll_For_SchoolClass_ShouldUpdate()
        {
            // Arrange
            // Set up the database sets and mocked School Class for create operations.
            var mockSettings = CreateDbSetSchoolClassMock();
            _dbMock.Setup(m => m.SchoolClasses).Returns(CreateDbSetSchoolClassMock().Object);
            _dbMock.Setup(m => m.Schools).Returns(CreateDbSetSchoolMock().Object);
            _dbMock.Setup(m => m.AspNetUsers).Returns(CreateDbSetUsers().Object);
            
            // Act

            await _schoolService.EnrollForClass("967146c6-4691-40d2-b917-a82188f83fc1", "e6fa8f57-d711-4e27-9678-a0b17b30b91a");

            // Assert

            var schoolClass = _dbMock.Object.SchoolClasses.First();

            // Verify that the SaveChangesAsync method on the SchoolService mock was called exactly once with the expected parameters.
            _dbMock.Verify(v => v.SaveChangesAsync(default), Times.Once);
            Assert.Equal(1, schoolClass.Students.Count());
        }
        [Fact]
        public async Task Withdraw_From_SchoolClass_ShouldUpdate()
        {
            // Arrange
            // Set up the database sets and mocked School Class for create operations.
            var mockSettings = CreateDbSetSchoolClassMock();
            _dbMock.Setup(m => m.SchoolClasses).Returns(CreateDbSetSchoolClassMock().Object);
            _dbMock.Setup(m => m.Schools).Returns(CreateDbSetSchoolMock().Object);
            _dbMock.Setup(m => m.AspNetUsers).Returns(CreateDbSetUsers().Object);

            // Act

            await _schoolService.WithdrawFromClass("967146c6-4691-40d2-b917-a82188f83fc1", "e6fa8f57-d711-4e27-9678-a0b17b30b91a");

            // Assert

            var schoolClass = _dbMock.Object.SchoolClasses.First();

            // Verify that the SaveChangesAsync method on the SchoolService mock was called exactly once with the expected parameters.
            _dbMock.Verify(v => v.SaveChangesAsync(default), Times.Once);
            Assert.Equal(0, schoolClass.Students.Count());
        }
        private static Mock<DbSet<School>> CreateDbSetSchoolMock()
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

        private static Mock<DbSet<SchoolClass>> CreateDbSetSchoolClassMock()
        {
            
            var schoolClass = new SchoolClass
            {
                Id = Guid.Parse("967146c6-4691-40d2-b917-a82188f83fc1"),
                Department = "testDepartment",
                HeadTeacher = new AspNetUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "testTeacher",
                    PasswordHash = "password".GetHashCode().ToString(),
                    FirstName = "testFirstName",
                    LastName = "testLastName",
                    SchoolId = "720acb13-c035-4f6f-a244-ff715df8c914"
                },
                School = new School
                {
                    Id = "720acb13-c035-4f6f-a244-ff715df8c914",
                    Address = "SomeAdress",
                    Name = "testSchool"
                },
                Year = 2023,
            };

            var dbSetMock = new Mock<DbSet<SchoolClass>>();
            var schools = new List<SchoolClass>() { schoolClass }.AsQueryable();

            dbSetMock.As<IQueryable<SchoolClass>>().Setup(m => m.Provider).Returns(schools.Provider);
            dbSetMock.As<IQueryable<SchoolClass>>().Setup(m => m.Expression).Returns(schools.Expression);
            dbSetMock.As<IQueryable<SchoolClass>>().Setup(m => m.ElementType).Returns(schools.ElementType);
            dbSetMock.As<IQueryable<SchoolClass>>().Setup(m => m.GetEnumerator()).Returns(schools.GetEnumerator());

            return dbSetMock;
        }

        private static Mock<DbSet<AspNetUser>> CreateDbSetUsers()
        {
            var teacher = new AspNetUser
            {
                Id = Guid.Parse("3cfa002b-7175-4900-9060-55a5c795ab50"),
                UserName = "testTeacherNew",
                FirstName = "testFirstNameNew",
                LastName = "testLastNameNew",
                SchoolId = "720acb13-c035-4f6f-a244-ff715df8c914"
            };

            var student = new AspNetUser
            {
                Id = Guid.Parse("e6fa8f57-d711-4e27-9678-a0b17b30b91a"),
                UserName = "testStudent",
                FirstName = "testStudentFirstName",
                LastName = "testStudentLastName",
                SchoolId = "720acb13-c035-4f6f-a244-ff715df8c914"
            };

            var usersMock = new Mock<DbSet<AspNetUser>>();
            var users = new List<AspNetUser> { teacher, student }.AsQueryable();
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            return usersMock;
        }
    }
}
