using GradeCenter.Data;
using Moq;
using Xunit;
using GradeCenter.Data.Models;
using GradeCenter.Services.Schools;
using Microsoft.EntityFrameworkCore;
using GradeCenter.Data.Models.Account;

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

            var users = new List<AspNetUser> {
                new AspNetUser
                {
                    FirstName = "John",
                    LastName = "Doe",
                    UserRole = UserRoles.Principle,
                },
                new AspNetUser
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    UserRole = UserRoles.Teacher
                },
                new AspNetUser
                {
                    FirstName = "Peter",
                    LastName = "Takell",
                    UserRole = UserRoles.Student
                },
            }.AsQueryable();

            var usersMock = new Mock<DbSet<AspNetUser>>();
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Set up the database mock to return the DbSet mock for Users
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

            var dbSetMock = new Mock<DbSet<School>>();
            var schools = new List<School>() { school }.AsQueryable();

            dbSetMock.As<IQueryable<School>>().Setup(m => m.Provider).Returns(schools.Provider);
            dbSetMock.As<IQueryable<School>>().Setup(m => m.Expression).Returns(schools.Expression);
            dbSetMock.As<IQueryable<School>>().Setup(m => m.ElementType).Returns(schools.ElementType);
            dbSetMock.As<IQueryable<School>>().Setup(m => m.GetEnumerator()).Returns(schools.GetEnumerator());

            _dbMock.Setup(x => x.Schools).Returns(dbSetMock.Object);

            _schoolService = new SchoolService(_dbMock.Object);
        }

        [Fact]
        public async Task Read_School_ReturnsEntries()
        {
            // Act
            var result = _schoolService.GetAllSchools();

            // Assert
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public async Task Create_School_SavesChanges()
        {
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
        public async Task AddPrincipleToSchool_ShouldUpdateSchool()
        {
            // Act
            var updatedSchool = new School
            {
                Id = "e74d4ee1-fe78-4390-a971-5d7080a5dbf6",
                Address = "UpdatedAddress",
                Name = "UpdatedName",
                People = new HashSet<AspNetUser>
                {
                    // Principal
                    new AspNetUser
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        UserRole = UserRoles.Principle
                    },
                    // Teacher
                    new AspNetUser
                    {
                        FirstName = "Stefany",
                        LastName = "Josith",
                        UserRole = UserRoles.Teacher
                    },
                }
            };

            await _schoolService.AddPrincipleToSchool(updatedSchool);

            var school = _dbMock.Object.Schools.First();

            // Assert
            // that the both schools have the same Principal
            Assert.Equal(school.People.First(x => x.UserRole == UserRoles.Principle), updatedSchool.People.First(x => x.UserRole == UserRoles.Principle));

            // Verify that the SaveChangesAsync method on the SchoolService mock was called exactly once with the expected parameters.
            _dbMock.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task AddTeacherToSchool_ShouldUpdateSchool()
        {
            // Act
            var updatedSchool = new School
            {
                Id = "e74d4ee1-fe78-4390-a971-5d7080a5dbf6",
                Address = "UpdatedAddress",
                Name = "UpdatedName",
                People = new HashSet<AspNetUser>
                {
                    new AspNetUser
                    {
                        FirstName = "Jane",
                        LastName = "Smith",
                        IsActive = true,
                        UserRole = UserRoles.Teacher
                    },
                    new AspNetUser
                    {
                        FirstName = "Peter",
                        LastName = "Collins",
                        IsActive = true,
                        UserRole = UserRoles.Teacher
                    },
                }
            };

            var school = _dbMock.Object.Schools.First();
            var numberOfTeachersBeforeUpdate = school.People.Where(x => x.UserRole == UserRoles.Teacher).ToList().ToArray().Length;

            await _schoolService.AddTeachersToSchool(updatedSchool);

            // Assert
            // that the number of teachers before the update is different
            Assert.NotEqual(numberOfTeachersBeforeUpdate, school.People.Where(x => x.UserRole == UserRoles.Teacher).ToList().ToArray().Length);

            // Verify that the SaveChangesAsync method on the SchoolService mock was called exactly once with the expected parameters.
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

        [Fact]
        public async Task AddStudentsToSchool_ShouldUpdateSchool()
        {
            // Act
            var updatedSchool = new School
            {
                Id = "e74d4ee1-fe78-4390-a971-5d7080a5dbf6",
                Address = "UpdatedAddress",
                Name = "UpdatedName",
                People = new HashSet<AspNetUser>
                {
                    new AspNetUser
                    {
                        FirstName = "Maria",
                        LastName = "Johnson",
                        IsActive = true,
                        UserRole = UserRoles.Student
                    },
                    new AspNetUser
                    {
                        FirstName = "Cate",
                        LastName = "Peterson",
                        IsActive = true,
                        UserRole = UserRoles.Student
                    },
                }
            };
            var school = _dbMock.Object.Schools.First();
            var numberOfStudentsBeforeUpdate = school.People.Where(x => x.UserRole == UserRoles.Student).ToList().ToArray().Length;

            await _schoolService.AddStudentsToSchool(updatedSchool);

            // Assert
            // that the number of students before the update is different
            Assert.NotEqual(numberOfStudentsBeforeUpdate, school.People.Where(x => x.UserRole == UserRoles.Student).ToList().ToArray().Length);

            // Verify that the SaveChangesAsync method on the SchoolService mock was called exactly once with the expected parameters.
            _dbMock.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteSchool_ShouldUpdateIsActive()
        {
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

    }
}
