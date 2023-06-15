using GradeCenter.Data;
using Moq;
using Xunit;
using GradeCenter.Data.Models;
using Microsoft.EntityFrameworkCore;
using GradeCenter.Data.Models.Account;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GradeCenter.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using GradeCenter.Services.interfaces;

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
            var userStoreMock = new Mock<IUserStore<AspNetUser>>();
            _userManagerMock = new Mock<UserManager<AspNetUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AspNetUser>>(_userManagerMock.Object, new Mock<IHttpContextAccessor>().Object, new Mock<IUserClaimsPrincipalFactory<AspNetUser>>().Object, null, null, null);
            _accountService = new AccountService(_userManagerMock.Object, _dbMock.Object, _signInManagerMock.Object);
            
            _schoolService = new SchoolService(_dbMock.Object, _accountService);
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
        public async Task UpdateSchool_ShouldUpdateAddress()
        {
            // Act
            var updatedSchool = new School
            {
                Id = "e74d4ee1-fe78-4390-a971-5d7080a5dbf6",
                Address = "UpdatedAddress",
                Name = "testSchool"
            };

            await _schoolService.Update(updatedSchool);

            var school = _dbMock.Object.Schools.First();

            // Assert
            // that the School Address is updated accordingly.
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
                Address = "testAddress",
                Name = "testSchool",
                People = new HashSet<AspNetUser>
                {
                    // Principal
                    new AspNetUser
                    {
                        FirstName = "OurNew",
                        LastName = "Principal",
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
                Address = "testAddress",
                Name = "testSchool",
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
        public async Task AddStudentsToSchool_ShouldUpdateSchool()
        {
            // Act
            var updatedSchool = new School
            {
                Id = "e74d4ee1-fe78-4390-a971-5d7080a5dbf6",
                Address = "testAddress",
                Name = "testSchool",
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

            // Verify that the SaveChangesAsync method on the SchoolService mock was called exactly twice - once for each student
            _dbMock.Verify(v => v.SaveChangesAsync(default), Times.Exactly(2));
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