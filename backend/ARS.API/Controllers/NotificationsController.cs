using ARS.Application.DTOs.Notifications;
using ARS.Domain.Entities;
using ARS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ARS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase  // ← Plural
{
    private readonly INotificationRepository _notificationRepository;

    // TODO: Reemplazar con userId del JWT token cuando implementemos auth
    private const string TempUserId = "69bdb6dbbd55a95504dea1a3";

    public NotificationsController(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetMyNotifications()
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(TempUserId);
        return Ok(notifications);
    }

    [HttpGet("unread")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetMyUnreadNotifications()
    {
        var notifications = await _notificationRepository.GetUnreadByUserIdAsync(TempUserId);
        return Ok(notifications); 
    }

    [HttpGet("unread/count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var count = await _notificationRepository.GetUnreadCountAsync(TempUserId);
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
        // Obtener la notificación
        var notification = await _notificationRepository.GetByIdAsync(id);

        if (notification == null)
            return NotFound($"Notification with ID {id} not found");

        // Verificar que pertenece al usuario actual
        if (notification.UserId != TempUserId)
        {
            //return Forbid();  // 403 Forbidden - no puedes marcar notificaciones de otros

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
        var marked = await _notificationRepository.MarkAllAsReadAsync(TempUserId);

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