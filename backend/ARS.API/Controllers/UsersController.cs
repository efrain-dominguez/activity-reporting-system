using ARS.Application.DTOs.Users;
using ARS.Domain.Entities;
using ARS.Domain.Enums;
using ARS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ARS.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return NotFound($"User with ID {id} not found");

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDto dto)
        {
            // Parse role (validation already ensured it's valid)
            if (!Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var userRole))
            {
                return BadRequest(new { error = "Invalid role" });
            }

            // Map DTO to Entity
            var user = new User
            {
                EntraObjectId = dto.EntraObjectId,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = userRole, 
                EntityId = dto.EntityId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }
    }
}