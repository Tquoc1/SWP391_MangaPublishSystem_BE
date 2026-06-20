using System;

namespace DTOs
{
    public class NotificationDto
    {
        public class NotificationResponse
        {
            public int NotificationId { get; set; }
            public int UserId { get; set; }
            public int? SeriesId { get; set; }
            public string Title { get; set; } = null!;
            public string Message { get; set; } = null!;
            public bool IsRead { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public class Create
        {
            public int UserId { get; set; }
            public int? SeriesId { get; set; }
            public string Title { get; set; } = null!;
            public string Message { get; set; } = null!;
        }
    }
}
