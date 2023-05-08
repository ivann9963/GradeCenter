using GradeCenter.Data;
using GradeCenter.Data.Models.Account;
using GradeCenter.Services;
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
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<GradeCenterContext> _dbMock;
        private readonly Mock<SignInManager<IdentityUser>> _signInManagerMock;
        private readonly IAccountService _accountService;

        public AccountServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _dbMock = new Mock<GradeCenterContext>();
            _signInManagerMock = new Mock<SignInManager<IdentityUser>>(_userManagerMock.Object, new Mock<IHttpContextAccessor>().Object, new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object, null, null, null);
            _accountService = new AccountService(_userManagerMock.Object, _dbMock.Object, _signInManagerMock.Object);
        }

        [Fact]
        public void Login_UserNotFound_ReturnsEmptyString()
        {
            // Arrange
            // Set up the DbSet mock for Users with an empty list of users
            var users = new List<User>().AsQueryable();
            var usersMock = new Mock<DbSet<User>>();
            usersMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Set up the database mock to return the DbSet mock for Users
            _dbMock.Setup(x => x.Users).Returns(usersMock.Object);

            // Act
            var result = _accountService.Login("test@example.com", "password");

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void Login_UserFound_ReturnsJwtToken()
        {
            // Arrange
            // Set up the DbSet mock for Users with a list containing one user
            var user = new User
            {
                UserName = "test@example.com",
            };

            var users = new List<User> { user }.AsQueryable();
            var usersMock = new Mock<DbSet<User>>();
            usersMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Set up the database mock to return the DbSet mock for Users
            _dbMock.Setup(x => x.Users).Returns(usersMock.Object);

            // Act
            var result = _accountService.Login("test@example.com", "password");

            // Assert
            Assert.NotEqual(string.Empty, result);
            Assert.True(result.Split('.').Length == 3);
        }

        [Fact]
        public async Task Register_CreatesUser_SavesChanges()
        {
            // Arrange
            // Set up the UserManager mock to return a successful IdentityResult
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            // Set up the database mock to return 1 (success) when SaveChangesAsync is called
            _dbMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            // Call the Register method on the AccountService object with sample data
            await _accountService.Register("userName", "test@example.com", "password");

            // Assert
            // Verify that the CreateAsync method on the UserManager mock was called exactly once with the expected parameters
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once());
            // Verify that the SaveChangesAsync method on the database mock was called exactly once
            _dbMock.Verify(x => x.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public void UpdateUser_ShouldUpdatePasswordAndPhoneNumber()
        {
            // Arrange
            var loggedUser = new User
            {
                Id = "1",
                UserName = "testUser",
                PasswordHash = "oldPassword".GetHashCode().ToString(),
                PhoneNumber = "1234567890"
            };

            string newPassword = "newPassword";
            string newPhoneNumber = "0987654321";

            // Act
            _accountService.UpdateUser(loggedUser, newPassword, newPhoneNumber);

            // Assert
            Assert.Equal(newPassword.GetHashCode().ToString(), loggedUser.PasswordHash);
            Assert.Equal(newPhoneNumber, loggedUser.PhoneNumber);
            _dbMock.Verify(db => db.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var loggedUser = new User
            {
                Id = "1",
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

