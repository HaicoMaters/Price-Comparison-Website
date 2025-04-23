using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceComparisonWebsite.Attributes;
using PriceComparisonWebsite.Services;

namespace PriceComparisonWebsite.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class NotificationApiController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationApiController> _logger;

        public NotificationApiController(
            INotificationService notificationService,
            ILogger<NotificationApiController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpPost("create-global-notification")]
        [InternalOrAuthorized("Admin")]
        public async Task<IActionResult> CreateGlobalNotification([FromBody] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("Attempted to create empty global notification");
                return BadRequest(new { success = false, message = "Message cannot be empty" });
            }

            try
            {
                await _notificationService.CreateGlobalNotification(message);
                return Ok(new { success = true, message = "Global notification sent successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating global notification");
                return StatusCode(500, new { success = false, message = $"Error sending notification: {ex.Message}" });
            }
        }

        [HttpGet("user-notifications/{userId}")]
        [InternalOrAuthorized("Admin,User")]
        public async Task<IActionResult> GetUserNotifications(string userId)
        {
            try
            {
                var readNotifications = await _notificationService.GetReadUserNotifications(userId);
                var unreadNotifications = await _notificationService.GetUnreadUserNotifications(userId);
                
                var allNotifications = readNotifications.Concat(unreadNotifications)
                    .GroupBy(n => n.Id)
                    .Select(g => g.First())
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new
                    {
                        id = n.Id,
                        message = n.Message,
                        timestamp = n.CreatedAt.ToString("g"),
                        isRead = !unreadNotifications.Any(un => un.Id == n.Id)
                    })
                    .ToList();

                return Ok(new { notifications = allNotifications });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching notifications for user {UserId}", userId);
                return StatusCode(500, new { success = false, message = $"Error fetching notifications: {ex.Message}" });
            }
        }

        [HttpPost("mark-as-read/{userId}")]
        [InternalOrAuthorized("Admin,User")]
        public async Task<IActionResult> MarkNotificationsAsRead(string userId)
        {
            try
            {
                await _notificationService.MarkNotificationsAsRead(userId);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notifications as read for user {UserId}", userId);
                return StatusCode(500, new { success = false, message = $"Error marking notifications as read: {ex.Message}" });
            }
        }

        [HttpPost("dismiss/{userId}/{notificationId}")]
        [InternalOrAuthorized("Admin,User")]
        public async Task<IActionResult> DismissNotification(string userId, int notificationId)
        {
            try
            {
                await _notificationService.DeleteUserNotification(notificationId, userId);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dismissing notification {NotificationId} for user {UserId}", notificationId, userId);
                return StatusCode(500, new { success = false, message = $"Error dismissing notification: {ex.Message}" });
            }
        }
    }
}