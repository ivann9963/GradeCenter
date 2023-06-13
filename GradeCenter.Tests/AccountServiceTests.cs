using GradeCenter.Data;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services;
using GradeCenter.Services.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GradeCenter.Tests
{
    public class AccountServiceTests
    {
        // Set up the necessary Mock objects and instantiate the AccountService object
        private readonly Mock<UserManager<AspNetUser>> _userManagerMock;
        private readonly Mock<GradeCenterContext> _dbMock;
        private readonly Mock<SignInManager<AspNetUser>> _signInManagerMock;
        private readonly IAccountService _accountService;

        public AccountServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<AspNetUser>>();
            _userManagerMock = new Mock<UserManager<AspNetUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _dbMock = new Mock<GradeCenterContext>();
            _signInManagerMock = new Mock<SignInManager<AspNetUser>>(_userManagerMock.Object, new Mock<IHttpContextAccessor>().Object, new Mock<IUserClaimsPrincipalFactory<AspNetUser>>().Object, null, null, null);
            _accountService = new AccountService(_userManagerMock.Object, _dbMock.Object, _signInManagerMock.Object);
        }

        [Fact]
        public async void Login_UserNotFound_ReturnsEmptyString()
        {
            // Arrange
            // Set up the DbSet mock for Users with an empty list of users
            var users = new List<AspNetUser>().AsQueryable();
            var usersMock = new Mock<DbSet<AspNetUser>>();
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Set up the database mock to return the DbSet mock for Users
            _dbMock.Setup(x => x.Users).Returns(usersMock.Object);

            // Act
            var result = await _accountService.Login("test@example.com", "password");

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async void Login_UserFound_ReturnsJwtToken()
        {
            // Arrange
            // Set up the DbSet mock for Users with a list containing one user

            var user = new AspNetUser
            {
                UserName = "test@example.com",
                PasswordHash = "password".GetHashCode().ToString()
            };

            var users = new List<AspNetUser> { user }.AsQueryable();
            var usersMock = new Mock<DbSet<AspNetUser>>();
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Set up the database mock to return the DbSet mock for Users
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<AspNetUser>(),"password")).ReturnsAsync(true);
            _dbMock.Setup(x => x.AspNetUsers).Returns(usersMock.Object);

            // Act
            var result = await _accountService.Login("test@example.com", "password");

            // Assert
            Assert.NotEqual(string.Empty, result);
            Assert.True(result.Split('.').Length == 3);
        }

        [Fact]
        public async Task Register_CreatesUser_SavesChanges()
        {
            // Arrange
            // Set up the UserManager mock to return a successful IdentityResult
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<AspNetUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            // Set up the database mock to return 1 (success) when SaveChangesAsync is called
            _dbMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            // Call the Register method on the AccountService object with sample data
            await _accountService.Register("userName", "test@example.com", "password");

            // Assert
            // Verify that the CreateAsync method on the UserManager mock was called exactly once with the expected parameters
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<AspNetUser>(), It.IsAny<string>()), Times.Once());
            // Verify that the SaveChangesAsync method on the database mock was called exactly once
            _dbMock.Verify(x => x.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public void UpdateUser_ShouldUpdatePasswordAndPhoneNumber()
        {
            // Arrange
            var loggedUser = new AspNetUser
            {
                Id = Guid.NewGuid(),
                UserName = "testUser",
                PasswordHash = "oldPassword".GetHashCode().ToString(),
                PhoneNumber = "1234567890",
                UserRole = UserRoles.Principle
            };

            var users = new List<AspNetUser> { loggedUser }.AsQueryable();
            var usersMock = new Mock<DbSet<AspNetUser>>();
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMock.As<IQueryable<AspNetUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Set up the database mock to return the DbSet mock for Users
            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<AspNetUser>(), "password")).ReturnsAsync(true);
            _dbMock.Setup(x => x.AspNetUsers).Returns(usersMock.Object);

            string newPassword = "newPassword";
            string newPhoneNumber = "0987654321";

            // Act
            _accountService.UpdateUser(loggedUser.Id.ToString(), newPassword, (UserRoles)loggedUser.UserRole, true ,newPhoneNumber);

            // Assert
            Assert.Equal(newPassword.GetHashCode().ToString(), loggedUser.PasswordHash);
            Assert.Equal(newPhoneNumber, loggedUser.PhoneNumber);
            Assert.Equal(UserRoles.Principle, loggedUser.UserRole);
            _dbMock.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var loggedUser = new AspNetUser
            {
                Id = Guid.NewGuid(),
                UserName = "testUser",
                PasswordHash = "password".GetHashCode().ToString(),
                PhoneNumber = "1234567890",
                IsActive = true
            };

            // Act
            _accountService.Deactivate(loggedUser);

            // Assert
            Assert.False(loggedUser.IsActive);
            _dbMock.Verify(db => db.SaveChanges(), Times.Once);
        }
    }
}

