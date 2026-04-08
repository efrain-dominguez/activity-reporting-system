using ARS.Application.DTOs.Notifications;
using ARS.Application.Services;
using ARS.Domain.Entities;
using ARS.Domain.Enums;
using ARS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ARS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController : BaseApiController
{
    private readonly INotificationRepository _notificationRepository;


    public NotificationsController(
         INotificationRepository notificationRepository,
         IUserRepository userRepository,
         ICurrentUserService currentUserService) : base(currentUserService, userRepository)
    {
        _notificationRepository = notificationRepository;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetMyNotifications()
    {
        var userId = await GetCurrentUserIdAsync();
        var notifications = await _notificationRepository.GetByUserIdAsync(userId);
        return Ok(notifications);
    }

    [HttpGet("unread")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetMyUnreadNotifications()
    {
        var userId = await GetCurrentUserIdAsync();
        var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);
        return Ok(notifications); 
    }

    [HttpGet("unread/count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userId = await GetCurrentUserIdAsync();
        var count = await _notificationRepository.GetUnreadCountAsync(userId);
        return Ok(count);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Notification>> GetNotificationById(string id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);

        if (notification == null)
            return NotFound($"Notification with ID {id} not found");

        return Ok(notification);
    }

    [HttpPatch("{id}/read")]
    public async Task<ActionResult> MarkAsRead(string id)
    {
        var userId = await GetCurrentUserIdAsync();
        // Obtener la notificación
        var notification = await _notificationRepository.GetByIdAsync(id);

        if (notification == null)
            return NotFound($"Notification with ID {id} not found");

        // Verify notification belongs to current user
        if (notification.UserId != userId)
            return Forbid();


        // Verificar que pertenece al usuario actual
        if (notification.UserId != userId)
        {

            return StatusCode(403, new
            {
                error = "Forbidden",
                message = "You do not have permission to mark this notification as read"
            });  
        }

        // Marcar como leída
        var marked = await _notificationRepository.MarkAsReadAsync(id);

        if (!marked)
            return StatusCode(500, "Failed to mark notification as read");

        return NoContent();  // 204 No Content
    }

    [HttpPatch("read-all")]
    public async Task<ActionResult> MarkAllAsRead()
    {
        var userId = await GetCurrentUserIdAsync();

        var marked = await _notificationRepository.MarkAllAsReadAsync(userId);

        if (!marked)
            return NotFound("No unread notifications found");

        return NoContent();  // 204 No Content
    }

    [HttpPost]
    public async Task<ActionResult<Notification>> CreateNotification([FromBody] CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            UserId = dto.UserId,
            Type = dto.Type,
            Title = dto.Title,
            Message = dto.Message,
            RelatedEntityType = dto.RelatedEntityType,
            RelatedEntityId = string.IsNullOrWhiteSpace(dto.RelatedEntityId) ? null : dto.RelatedEntityId, 
            IsRead = false,
            SentViaEmail = false,
            CreatedAt = DateTime.UtcNow
        };

        var createdNotification = await _notificationRepository.CreateAsync(notification);
        return CreatedAtAction(nameof(GetNotificationById), new { id = createdNotification.Id }, createdNotification);
    }
}