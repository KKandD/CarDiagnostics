using CarDiagnostics.Application.DTOs.Users;
using CarDiagnostics.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDiagnostics.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var users = await _userService.GetPagedUsers(pageNumber, pageSize);

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching users.");
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", id);
                    return NotFound(new { message = "User not found." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching user with ID {UserId}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid input for updating user with ID {UserId}.", id);
                return BadRequest(new { message = "Invalid input.", details = ModelState });
            }

            try
            {
                var updated = await _userService.UpdateUser(id, updateUserDto);
                if (!updated)
                {
                    _logger.LogWarning("User with ID {UserId} not found for update.", id);
                    return NotFound(new { message = "User not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating user with ID {UserId}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {

                var deleted = await _userService.DeleteUser(id);
                if (!deleted)
                {
                    _logger.LogWarning("User with ID {UserId} not found for deletion.", id);
                    return NotFound(new { message = "User not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting user with ID {UserId}.", id);
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
