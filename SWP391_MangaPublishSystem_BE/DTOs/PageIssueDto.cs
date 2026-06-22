using System;
using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public class PageIssueDto
    {
        public int Issueid { get; set; }
        public int Pageid { get; set; }
        public int CreatedById { get; set; }
        public int? AssignedToId { get; set; }
        public string IssueType { get; set; }
        public string WorkCategory { get; set; }

        public int BoxX { get; set; }
        public int BoxY { get; set; }
        public int BoxWidth { get; set; }
        public int BoxHeight { get; set; }

        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? Createdat { get; set; }
        public DateTime? Completedat { get; set; }
        public bool? Isdeleted { get; set; }

        public class Create
        {
            [Required(ErrorMessage = "Page ID không được để trống")]
            public int Pageid { get; set; }

            [Required(ErrorMessage = "Người tạo không được để trống")]
            public int CreatedById { get; set; }

            public int? AssignedToId { get; set; }

            [Required(ErrorMessage = "Loại vấn đề không được để trống")]
            public string IssueType { get; set; }

            [Required(ErrorMessage = "Danh mục công việc không được để trống")]
            public string WorkCategory { get; set; }

            public int BoxX { get; set; }
            public int BoxY { get; set; }
            public int BoxWidth { get; set; }
            public int BoxHeight { get; set; }

            [Required(ErrorMessage = "Mô tả không được để trống")]
            public string Description { get; set; }
            public DateTime? Deadline { get; set; }
        }

        public class Update
        {
            public int? AssignedToId { get; set; }
            public string Description { get; set; }

            public int BoxX { get; set; }
            public int BoxY { get; set; }
            public int BoxWidth { get; set; }
            public int BoxHeight { get; set; }

            public DateTime? Deadline { get; set; }
            public DateTime? Completedat { get; set; }
        }
        public class UpdateStatus
        {
            [Required(ErrorMessage = "Trạng thái không được để trống")]
            public string Status { get; set; } = string.Empty;
        }
    }
}
