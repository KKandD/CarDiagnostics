using CarDiagnostics.Application.DTOs.Users;
using CarDiagnostics.Application.Interfaces.Repositories;
using CarDiagnostics.Application.Services;
using CarDiagnostics.Domain.Entities.Users;
using Common.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarDiagnostics.Tests.Services
{
    public class AuthorizationServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<AuthorizationService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthorizationService _authorizationService;

        public AuthorizationServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<AuthorizationService>>();
            _configurationMock = new Mock<IConfiguration>();

            // Mocking JWT settings
            _configurationMock.Setup(config => config["Jwt:Key"]).Returns("ThisIsASecretKeyForTestingPurposesOnly123!");
            _configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(config => config["Jwt:Audience"]).Returns("TestAudience");

            _authorizationService = new AuthorizationService(
                _userRepositoryMock.Object,
                _configurationMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Register_ShouldReturnToken_WhenUserIsCreated()
        {
            // Arrange
            var createUserDto = new CreateUserDto { Email = "test@example.com", Password = "Password123" };
            _userRepositoryMock.Setup(repo => repo.GetByEmail(createUserDto.Email)).ReturnsAsync((UserEntity)null);
            _userRepositoryMock.Setup(repo => repo.AddUser(It.IsAny<UserEntity>())).Returns(Task.CompletedTask);

            // Act
            var token = await _authorizationService.Register(createUserDto);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task Register_ShouldThrowException_WhenUserAlreadyExists()
        {
            // Arrange
            var createUserDto = new CreateUserDto { Email = "existing@example.com", Password = "Password123" };
            var existingUser = new UserEntity { Email = "existing@example.com", Password = "HashedPassword" };
            _userRepositoryMock.Setup(repo => repo.GetByEmail(createUserDto.Email)).ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authorizationService.Register(createUserDto));
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var createUserDto = new CreateUserDto { Email = "test@example.com", Password = "Password123" };
            var existingUser = new UserEntity { Email = "test@example.com", Password = PasswordHelper.HashPassword("Password123") };
            _userRepositoryMock.Setup(repo => repo.GetByEmail(createUserDto.Email)).ReturnsAsync(existingUser);

            // Act
            var token = await _authorizationService.Login(createUserDto);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task Login_ShouldThrowException_WhenInvalidEmailOrPassword()
        {
            // Arrange
            var createUserDto = new CreateUserDto { Email = "test@example.com", Password = "WrongPassword" };
            var existingUser = new UserEntity { Email = "test@example.com", Password = PasswordHelper.HashPassword("CorrectPassword") };
            _userRepositoryMock.Setup(repo => repo.GetByEmail(createUserDto.Email)).ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authorizationService.Login(createUserDto));
        }

        [Fact]
        public void GenerateJwtToken_ShouldContainEmailClaim()
        {
            // Arrange
            var user = new UserEntity { Email = "test@example.com" };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationMock.Object["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configurationMock.Object["Jwt:Issuer"],
                audience: _configurationMock.Object["Jwt:Audience"],
                claims: new[] { new Claim(ClaimTypes.Email, user.Email) },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Act
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            // Assert
            Assert.Equal(user.Email, emailClaim);
        }
    }
}
