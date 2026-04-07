using ARS.Application.DTOs.Activities;
using ARS.Domain.Entities;
using ARS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ARS.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityRepository _activityRepository;

        // TODO: Obtener del JWT cuando implementemos autenticación
        private const string TempUserId = "67460f8a1c2d3e4f5a6b7c8d";

        public ActivitiesController(IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        #region CRUD_BASICO

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Activity>>> GetAllActivities()
        {
            var activities = await _activityRepository.GetAllAsync();
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivityById(string id)
        {
            var activity = await _activityRepository.GetByIdAsync(id);

            if (activity == null)
                return NotFound($"Activity with ID {id} not found");

            return Ok(activity);
        }

        [HttpPost]
        public async Task<ActionResult<Activity>> CreateActivity([FromBody] CreateActivityDto dto)
        {
            var activity = new Activity
            {
                AssignmentId = dto.AssignmentId,
                RequestId = dto.RequestId,
                Description = dto.Description,
                ActivityDate = dto.ActivityDate,
                SubmittedByUserId = TempUserId,
                IsEditable = true,
                Files = new List<ActivityFile>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow 
            };

            var createdActivity = await _activityRepository.CreateAsync(activity);
            return CreatedAtAction(nameof(GetActivityById), new { id = createdActivity.Id }, createdActivity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateActivity(string id, [FromBody] CreateActivityDto dto)
        {
            var activity = await _activityRepository.GetByIdAsync(id);

            if (activity == null)
                return NotFound($"Activity with ID {id} not found");

            if (!activity.IsEditable)
                return BadRequest("Activity has already been submitted and cannot be modified");

            // Actualizar campos
            activity.Description = dto.Description;
            activity.ActivityDate = dto.ActivityDate;
            activity.UpdatedAt = DateTime.UtcNow;

            var updated = await _activityRepository.UpdateAsync(id, activity);

            if (!updated)
                return StatusCode(500, "Failed to update activity");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivity(string id)
        {
            var activity = await _activityRepository.GetByIdAsync(id);

            if (activity == null)
                return NotFound($"Activity with ID {id} not found");

            if (!activity.IsEditable)
                return BadRequest("Activity has already been submitted and cannot be deleted");

            var deleted = await _activityRepository.DeleteAsync(id);

            if (!deleted)
                return StatusCode(500, "Failed to delete activity");

            return NoContent();
        }

        #endregion

        #region FILTROS_ESPECIFICOS

        [HttpGet("assignment/{assignmentId}")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivitiesByAssignment(string assignmentId)
        {
            var activities = await _activityRepository.GetByAssignmentIdAsync(assignmentId);
            return Ok(activities);
        }

        [HttpGet("request/{requestId}")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivitiesByRequest(string requestId)
        {
            var activities = await _activityRepository.GetByRequestIdAsync(requestId);
            return Ok(activities);
        }

        [HttpGet("my-activities")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetMyActivities()
        {
            var activities = await _activityRepository.GetBySubmittedByUserIdAsync(TempUserId);
            return Ok(activities);
        }

        [HttpGet("editable")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetEditableActivities()
        {
            var activities = await _activityRepository.GetEditableActivitiesAsync();
            return Ok(activities);
        }

        #endregion

        #region ACCIONES_WORKFLOW

        [HttpPatch("{id}/submit")]
        public async Task<ActionResult> SubmitActivity(string id)
        {
            var activity = await _activityRepository.GetByIdAsync(id);

            if (activity == null)
                return NotFound($"Activity with ID {id} not found");

            if (!activity.IsEditable)
                return BadRequest("Activity has already been submitted and cannot be modified");

            activity.Submit(); 
            activity.SubmittedAt = DateTime.UtcNow; 
            activity.UpdatedAt = DateTime.UtcNow;

            var updated = await _activityRepository.UpdateAsync(id, activity);

            if (!updated)
                return StatusCode(500, "Failed to submit activity");

            return NoContent();
        }

        #endregion
    }
}