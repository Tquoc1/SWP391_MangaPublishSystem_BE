using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class NotificationRepository : GenericRepository<Notification>
    {
        public NotificationRepository() { }

        public NotificationRepository(MangaPublishDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.Userid == userId)
                .OrderByDescending(n => n.Createdat)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.Userid == userId && (n.Isread == false || n.Isread == null));
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.Userid == userId && (n.Isread == false || n.Isread == null))
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.Isread = true;
            }

            if (unreadNotifications.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
