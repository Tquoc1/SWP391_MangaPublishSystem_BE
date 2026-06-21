using DTOs;
using Entities.Models;
using Repositories.Repository;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationRepository _notificationRepository;

        public NotificationService(NotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<NotificationDto.NotificationResponse>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUserNotificationsAsync(userId);
            
            return notifications.Select(n => new NotificationDto.NotificationResponse
            {
                NotificationId = n.Notificationid,
                UserId = n.Userid,
                SeriesId = n.Seriesid,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.Isread ?? false,
                CreatedAt = n.Createdat
            }).ToList();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null || notification.Userid != userId)
            {
                return false;
            }

            notification.Isread = true;
            await _notificationRepository.UpdateAsync(notification);
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
            return true;
        }

        public async Task<bool> CreateNotificationAsync(int userId, string title, string message, int? seriesId = null)
        {
            var notification = new Notification
            {
                Userid = userId,
                Title = title,
                Message = message,
                Seriesid = seriesId,
                Isread = false,
                Createdat = DateTime.UtcNow
            };

            await _notificationRepository.CreateAsync(notification);
            return true;
        }
    }
}
