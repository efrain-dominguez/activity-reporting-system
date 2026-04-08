using ARS.Application.DTOs.Entities;
using ARS.Application.Services;
using ARS.Domain.Entities;
using ARS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ARS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EntitiesController : BaseApiController
    {

        private readonly IEntityRepository _entityRepository;

        public EntitiesController(IEntityRepository entityRepository, ICurrentUserService currentUserService, IUserRepository userRepository) : base(currentUserService, userRepository)
        {
            _entityRepository = entityRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entity>>> GetAllEntities()
        {
            var entities = await _entityRepository.GetActiveEntitiesAsync();
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Entity>> GetEntityById(string id)
        {
            var entity = await _entityRepository.GetByIdAsync(id);

            if (entity == null)
                return NotFound($"Entity with ID {id} not found");

            return Ok(entity);
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<Entity>> GetEntityByCode(string code)
        {
            var entity = await _entityRepository.GetByCodeAsync(code);

            if (entity == null)
                return NotFound($"Entity with Code {code} not found");

            return Ok(entity);
        }


        [HttpGet("children/{parentId}")]
        public async Task<ActionResult<IEnumerable<Entity>>> GetChildEntities(string parentId)
        {
            var entities = await _entityRepository.GetByParentEntityIdAsync(parentId);

            if (!entities.Any())
                return NotFound($"No child entities found for parent {parentId}");

            return Ok(entities);
        }

        [HttpPost]
        public async Task<ActionResult<Entity>> CreateEntity([FromBody] CreateEntityDto dto)
        {
            // Verificar si el código ya existe
            var existingEntity = await _entityRepository.GetByCodeAsync(dto.Code);
            if (existingEntity != null)
            {
                return Conflict($"Entity with code '{dto.Code}' already exists");
            }

            // Map DTO to Entity
            var entity = new Entity
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                ParentEntityId = string.IsNullOrWhiteSpace(dto.ParentEntityId) ? null : dto.ParentEntityId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,   
                UpdatedAt = DateTime.UtcNow   
            };

            var createdEntity = await _entityRepository.CreateAsync(entity);
            return CreatedAtAction(nameof(GetEntityById), new { id = createdEntity.Id }, createdEntity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateEntity(string id, [FromBody] UpdateEntityDto dto)
        {

            var entity = await _entityRepository.GetByIdAsync(id);

            if (entity == null)
                return NotFound($"Entity with Code {id} not found");


            entity.Name = dto.Name;
            entity.Description = dto.Description;
            // En CreateEntity y UpdateEntity
            entity.ParentEntityId = string.IsNullOrWhiteSpace(dto.ParentEntityId) ? null : dto.ParentEntityId;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            var updated = await _entityRepository.UpdateAsync(id, entity);

            if (!updated)
                return StatusCode(500, "Failed to update entity");  // Error interno

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEntity(string id)
        {

            var entity = await _entityRepository.GetByIdAsync(id);

            if (entity == null)
                return NotFound($"Entity with Code {id} not found");


            var deleted = await _entityRepository.DeleteAsync(id);

            if (!deleted)
                return StatusCode(500, "Failed to delete entity");

            return NoContent();  // 204 es estándar para DELETE exitoso
        }
    }
}
