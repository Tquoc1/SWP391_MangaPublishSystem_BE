using System;

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
            public DateTime? Deadline { get; set; }
        }

        public class Update
        {
            public int? AssignedToId { get; set; }
            public string Status { get; set; }
            public string Description { get; set; }

            public int BoxX { get; set; }
            public int BoxY { get; set; }
            public int BoxWidth { get; set; }
            public int BoxHeight { get; set; }

            public DateTime? Deadline { get; set; }
            public DateTime? Completedat { get; set; }
            public bool? Isdeleted { get; set; }
        }
        public class UpdateStatus
        {
            public string Status { get; set; } = string.Empty;
        }
    }
}
