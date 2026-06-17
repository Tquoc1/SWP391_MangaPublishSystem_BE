using System;

namespace DTOs
{
    public class ChapterDto
    {
        public int Chapterid { get; set; }
        public int Seriesid { get; set; }
        public int Chapternumber { get; set; }
        public string Title { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public DateTime? Createdat { get; set; }
        public bool? Isdeleted { get; set; }

        public class Create
        {
            public int Seriesid { get; set; }
            public int Chapternumber { get; set; }
            public string Title { get; set; }
            public DateTime Deadline { get; set; }
        }

        public class Update
        {
            [System.Text.Json.Serialization.JsonIgnore]
            public int Chapterid { get; set; }

            public int Chapternumber { get; set; }
            public string Title { get; set; }
            public DateTime Deadline { get; set; }
            //public string Status { get; set; }
            //public bool? Isdeleted { get; set; }
        }
    }
}
