using System;
using System.ComponentModel.DataAnnotations;

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
            [Required(ErrorMessage = "User ID không được để trống")]
            public int UserId { get; set; }

            public int? SeriesId { get; set; }

            [Required(ErrorMessage = "Tiêu đề không được để trống")]
            [MaxLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
            public string Title { get; set; } = null!;

            [Required(ErrorMessage = "Nội dung không được để trống")]
            public string Message { get; set; } = null!;
        }
    }
}
