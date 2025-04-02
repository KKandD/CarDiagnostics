using CarDiagnostics.Application.DTOs.Users;
using CarDiagnostics.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace CarDiagnostics.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(IAuthorizationService authorizationService, ILogger<AuthorizationController> logger)
        {
            _authorizationService = authorizationService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid registration data provided for email {Email}.", createUserDto.Email);
                    return BadRequest(new { Error = "Invalid input data.", Details = ModelState });
                }

                var token = await _authorizationService.Register(createUserDto);
                _logger.LogInformation("User {Email} registered successfully.", createUserDto.Email);

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration attempt for email {Email}.", createUserDto.Email);
                return BadRequest(new { Error = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid login data provided for email {Email}.", createUserDto.Email);
                    return BadRequest(new { Error = "Invalid input data.", Details = ModelState });
                }

                var token = await _authorizationService.Login(createUserDto);
                _logger.LogInformation("User {Email} logged in successfully.", createUserDto.Email);

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login attempt for email {Email}.", createUserDto.Email);
                return Unauthorized(new { Error = ex.Message });
            }
        }
    }
}
