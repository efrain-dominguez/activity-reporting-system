using ARS.Application.DTOs.RequestAssignments;
using ARS.Domain.Entities;
using ARS.Domain.Enums;
using ARS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ARS.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RequestAssignmentsController : ControllerBase
    {
        private readonly IRequestAssignmentRepository _requestAssignmentRepository;

        // TODO: Obtener del JWT cuando implementemos autenticación
        private const string TempUserId = "67460f8a1c2d3e4f5a6b7c8d";

        public RequestAssignmentsController(IRequestAssignmentRepository requestAssignmentRepository)
        {
            _requestAssignmentRepository = requestAssignmentRepository;
        }

        #region CRUD_BASICO

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestAssignment>>> GetAllAssignments()
        {
            var requestAssignments = await _requestAssignmentRepository.GetAllAsync();
            return Ok(requestAssignments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequestAssignment>> GetAssignmentById(string id)
        {
            var requestAssignment = await _requestAssignmentRepository.GetByIdAsync(id);

            if (requestAssignment == null)
                return NotFound($"Request assignment with ID {id} not found");

            return Ok(requestAssignment);
        }

        [HttpPost]
        public async Task<ActionResult<RequestAssignment>> CreateAssignment([FromBody] CreateRequestAssignmentDto dto)
        {
            var requestAssignment = new RequestAssignment
            {
                RequestId = dto.RequestId,
                AssignedToEntityId = dto.AssignedToEntityId,
                AssignedToUserId = dto.AssignedToUserId,
                AssignedByUserId = TempUserId,
                DelegatedFromUserId = dto.DelegatedFromUserId,
                Status = AssignmentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow  // ← AGREGADO
            };

            var createdRequestAssignment = await _requestAssignmentRepository.CreateAsync(requestAssignment);
            return CreatedAtAction(nameof(GetAssignmentById), new { id = createdRequestAssignment.Id }, createdRequestAssignment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAssignment(string id, [FromBody] UpdateRequestAssignmentDto dto)
        {
            var requestAssignment = await _requestAssignmentRepository.GetByIdAsync(id);

            if (requestAssignment == null)
                return NotFound($"Request assignment with ID {id} not found");

            // Solo Pending/InProgress pueden editarse
            if (requestAssignment.Status != AssignmentStatus.Pending &&
                requestAssignment.Status != AssignmentStatus.InProgress)
                return BadRequest($"Cannot update assignment with status '{requestAssignment.Status}'. Only Pending or InProgress assignments can be updated.");

            // Validar que si el NUEVO status es NoProgress, debe tener razón
            if (dto.Status == AssignmentStatus.NoProgress && string.IsNullOrWhiteSpace(dto.NoProgressReason))
                return BadRequest("NoProgressReason is required when status is NoProgress");

            // Actualizar campos
            requestAssignment.Status = dto.Status;
            requestAssignment.NoProgressReason = dto.NoProgressReason;

            // Solo actualizar fecha si se está solicitando extensión por primera vez
            if (dto.ExtensionRequested && !requestAssignment.ExtensionRequested)
            {
                requestAssignment.ExtensionRequestedDate = DateTime.UtcNow;
            }
            requestAssignment.ExtensionRequested = dto.ExtensionRequested;

            requestAssignment.UpdatedAt = DateTime.UtcNow;

            var updated = await _requestAssignmentRepository.UpdateAsync(id, requestAssignment);

            if (!updated)
                return StatusCode(500, "Failed to update assignment");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAssignment(string id)
        {
            var requestAssignment = await _requestAssignmentRepository.GetByIdAsync(id);

            if (requestAssignment == null)
                return NotFound($"Request assignment with ID {id} not found");

            var deleted = await _requestAssignmentRepository.DeleteAsync(id);

            if (!deleted)
                return StatusCode(500, "Failed to delete request assignment");

            return NoContent();
        }

        #endregion
    }
}