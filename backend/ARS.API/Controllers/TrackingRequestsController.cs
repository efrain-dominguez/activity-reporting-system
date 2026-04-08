using ARS.Application.DTOs.TrackingRequests;
using ARS.Application.Services;
using ARS.Domain.Entities;
using ARS.Domain.Enums;
using ARS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace ARS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TrackingRequestsController : BaseApiController
{
    private readonly ITrackingRequestRepository _trackingRequestRepository;


    public TrackingRequestsController(ITrackingRequestRepository trackingRequestRepository, IUserRepository userRepository,
         ICurrentUserService currentUserService) : base(currentUserService, userRepository)
    {
        _trackingRequestRepository = trackingRequestRepository;
    }

    #region CRUD Básico

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TrackingRequest>>> GetAllRequests()
    {
        var trackingRequests = await _trackingRequestRepository.GetAllAsync();
        return Ok(trackingRequests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TrackingRequest>> GetRequestById(string id)
    {
        var trackingRequest = await _trackingRequestRepository.GetByIdAsync(id);

        if (trackingRequest == null)
            return NotFound($"Tracking request with ID {id} not found");

        return Ok(trackingRequest);
    }

    [HttpPost]
    public async Task<ActionResult<TrackingRequest>> CreateRequest([FromBody] CreateTrackingRequestDto dto)
    {
        var trackingRequest = new TrackingRequest
        {
            Title = dto.Title,
            Description = dto.Description,
            GoalType = dto.GoalType,
            CreatedByUserId = await GetCurrentUserIdAsync(),
            TargetEntityIds = dto.TargetEntityIds,
            StartDate = dto.StartDate,
            DueDate = dto.DueDate,
            Frequency = dto.Frequency,
            Status = RequestStatus.Draft,
            IsRecurring = dto.IsRecurring,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdTrackingRequest = await _trackingRequestRepository.CreateAsync(trackingRequest);
        return CreatedAtAction(nameof(GetRequestById), new { id = createdTrackingRequest.Id }, createdTrackingRequest);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateRequest(string id, [FromBody] UpdateTrackingRequestDto dto)
    {
        var trackingRequest = await _trackingRequestRepository.GetByIdAsync(id);

        if (trackingRequest == null)
            return NotFound($"Tracking request with ID {id} not found");

        // Solo Draft/Pending pueden editarse
        if (trackingRequest.Status != RequestStatus.Draft && trackingRequest.Status != RequestStatus.Pending)
            return BadRequest($"Cannot update request with status '{trackingRequest.Status}'. Only Draft or Pending requests can be updated.");

        // Validar que DueDate sea después de StartDate
        if (dto.DueDate <= trackingRequest.StartDate)
            return BadRequest("Due date must be after start date");

        // Actualizar campos
        trackingRequest.Title = dto.Title;
        trackingRequest.Description = dto.Description;
        trackingRequest.GoalType = dto.GoalType;
        trackingRequest.TargetEntityIds = dto.TargetEntityIds;
        trackingRequest.StartDate = dto.StartDate;
        trackingRequest.DueDate = dto.DueDate;
        trackingRequest.Frequency = dto.Frequency;
        trackingRequest.IsRecurring = dto.IsRecurring;
        trackingRequest.Status = dto.Status; 
        trackingRequest.UpdatedAt = DateTime.UtcNow;

        var updated = await _trackingRequestRepository.UpdateAsync(id, trackingRequest);

        if (!updated)
            return StatusCode(500, "Failed to update tracking request");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRequest(string id)
    {
        var trackingRequest = await _trackingRequestRepository.GetByIdAsync(id);

        if (trackingRequest == null)
            return NotFound($"Tracking request with ID {id} not found");

        // Solo Draft pueden eliminarse
        if (trackingRequest.Status != RequestStatus.Draft)
            return BadRequest("Only requests in Draft status can be deleted");

        var deleted = await _trackingRequestRepository.DeleteAsync(id);

        if (!deleted)
            return StatusCode(500, "Failed to delete tracking request");

        return NoContent();
    }

    #endregion

    #region Filtros Específicos

    [HttpGet("my-requests")]
    public async Task<ActionResult<IEnumerable<TrackingRequest>>> GetMyRequests()
    {
        var userId = await GetCurrentUserIdAsync();
        var trackingRequests = await _trackingRequestRepository.GetByCreatedByUserIdAsync(userId);
        return Ok(trackingRequests);
    }

    [HttpGet("by-entity/{entityId}")]
    public async Task<ActionResult<IEnumerable<TrackingRequest>>> GetRequestsByEntity(string entityId)
    {
        var trackingRequests = await _trackingRequestRepository.GetByTargetEntityIdAsync(entityId);
        return Ok(trackingRequests);
    }

    [HttpGet("by-status/{status}")]
    public async Task<ActionResult<IEnumerable<TrackingRequest>>> GetRequestsByStatus(string status)
    {
        if (!Enum.TryParse<RequestStatus>(status, ignoreCase: true, out var requestStatus))
        {
            return BadRequest($"Invalid status. Valid values: {string.Join(", ", Enum.GetNames(typeof(RequestStatus)))}");
        }

        var trackingRequests = await _trackingRequestRepository.GetByStatusAsync(requestStatus);
        return Ok(trackingRequests);
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<TrackingRequest>>> GetOverdueRequests()
    {
        var trackingRequests = await _trackingRequestRepository.GetOverdueRequestsAsync();
        return Ok(trackingRequests);
    }

    [HttpGet("in-progress")]
    public async Task<ActionResult<IEnumerable<TrackingRequest>>> GetInProgressRequests()
    {
        var trackingRequests = await _trackingRequestRepository.GetInProgressRequestsAsync();
        return Ok(trackingRequests);
    }

    #endregion

    #region Cambios de Estado

    [HttpPatch("{id}/complete")]
    public async Task<ActionResult> CompleteRequest(string id)
    {
        var request = await _trackingRequestRepository.GetByIdAsync(id);

        if (request == null)
            return NotFound($"Tracking request with ID {id} not found");

        if (request.Status == RequestStatus.Overdue)
            return BadRequest("Cannot complete an overdue request");

        if (request.Status == RequestStatus.Completed)
            return BadRequest("Request is already completed");

        request.Status = RequestStatus.Completed;
        request.UpdatedAt = DateTime.UtcNow;

        var updated = await _trackingRequestRepository.UpdateAsync(id, request);

        if (!updated)
            return StatusCode(500, "Failed to complete request");

        return NoContent();
    }

    [HttpPatch("{id}/overdue")]
    public async Task<ActionResult> MarkAsOverdue(string id)
    {
        var marked = await _trackingRequestRepository.MarkAsOverdueAsync(id);

        if (!marked)
            return NotFound($"Tracking request with ID {id} not found or cannot be marked as overdue");

        return NoContent();
    }

    #endregion
}