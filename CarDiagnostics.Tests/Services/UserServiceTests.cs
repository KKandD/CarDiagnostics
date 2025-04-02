using CarDiagnostics.Application.DTOs.Users;
using CarDiagnostics.Application.Interfaces.Repositories;
using CarDiagnostics.Application.Services;
using CarDiagnostics.Domain.Entities.Users;
using Moq;

namespace CarDiagnostics.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetPagedUsers_ShouldReturnPaginatedListOfUsers()
        {
            // Arrange
            var users = new List<UserEntity>
            {
                new UserEntity { Id = 1, Email = "user1@example.com" },
                new UserEntity { Id = 2, Email = "user2@example.com" },
                new UserEntity { Id = 3, Email = "user3@example.com" },
                new UserEntity { Id = 4, Email = "user4@example.com" }
            };

            int pageNumber = 1;
            int pageSize = 2;

            var paginatedUsers = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            _userRepositoryMock.Setup(repo => repo.GetPagedUsers(pageNumber, pageSize))
                .ReturnsAsync(paginatedUsers);

            _userRepositoryMock.Setup(repo => repo.GetTotalUserCount())
                .ReturnsAsync(users.Count);

            // Act
            var result = await _userService.GetPagedUsers(pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pageSize, result.Items.Count());
            Assert.Equal(users.Count, result.TotalCount);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);

            Assert.Contains(result.Items, u => u.Email == "user1@example.com");
            Assert.Contains(result.Items, u => u.Email == "user2@example.com");
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new UserEntity { Id = 1, Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetByEmail("test@example.com")).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByEmail("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmail_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetByEmail("nonexistent@example.com")).ReturnsAsync((UserEntity)null);

            // Act
            var result = await _userService.GetUserByEmail("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new UserEntity { Id = 1, Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetById(1)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.GetById(99)).ReturnsAsync((UserEntity)null);

            // Act
            var result = await _userService.GetUserById(99);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnTrue_WhenUserExists()
        {
            // Arrange
            var user = new UserEntity { Id = 1, Email = "old@example.com", Password = "OldPassword" };
            var updateDto = new UpdateUserDto { Email = "new@example.com", Password = "NewPassword123" };

            _userRepositoryMock.Setup(repo => repo.GetById(1)).ReturnsAsync(user);
            _userRepositoryMock.Setup(repo => repo.UpdateUser(It.IsAny<UserEntity>())).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUser(1, updateDto);

            // Assert
            Assert.True(result);
            _userRepositoryMock.Verify(repo => repo.UpdateUser(It.Is<UserEntity>(u => u.Email == "new@example.com")), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateUserDto { Email = "new@example.com", Password = "NewPassword123" };
            _userRepositoryMock.Setup(repo => repo.GetById(99)).ReturnsAsync((UserEntity)null);

            // Act
            var result = await _userService.UpdateUser(99, updateDto);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.UpdateUser(It.IsAny<UserEntity>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnTrue()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.DeleteUser(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.DeleteUser(1);

            // Assert
            Assert.True(result);
            _userRepositoryMock.Verify(repo => repo.DeleteUser(1), Times.Once);
        }
    }
}
