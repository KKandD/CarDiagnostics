using CarDiagnostics.Application.DTOs.Users;
using CarDiagnostics.Application.Interfaces.Repositories;
using CarDiagnostics.Application.Interfaces.Services;
using CarDiagnostics.Domain.Entities.Users;
using Common.Helpers;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarDiagnostics.Application.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthorizationService> _logger;

        public AuthorizationService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthorizationService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> Register(CreateUserDto createUserDto)
        {
            try
            {
                if (await _userRepository.GetByEmail(createUserDto.Email) != null)
                    throw new Exception("Email already exists");

                var user = new UserEntity
                {
                    Email = createUserDto.Email,
                    Password = PasswordHelper.HashPassword(createUserDto.Password)
                };

                await _userRepository.AddUser(user);

                string token = GenerateJwtToken(user);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<string> Login(CreateUserDto createUserDto)
        {
            try
            {
                var user = await _userRepository.GetByEmail(createUserDto.Email);
                if (user == null || !PasswordHelper.VerifyPassword(createUserDto.Password, user.Password))
                    throw new Exception("Invalid email or password");

                string token = GenerateJwtToken(user);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        private string GenerateJwtToken(UserEntity user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: new[] { new Claim(ClaimTypes.Email, user.Email) },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
