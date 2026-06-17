using System;

namespace DTOs
{
    public class PageDto
    {
        public int Pageid { get; set; }
        public int Chapterid { get; set; }
        public int Pagenumber { get; set; }
        public string Pageimageurl { get; set; }
        public string Status { get; set; }
        public bool? Isdeleted { get; set; }

        public class Create
        {
            public int Chapterid { get; set; }
            public int Pagenumber { get; set; }
        }

        public class Update
        {
            public int Pagenumber { get; set; }
            //public string Status { get; set; }
            //public bool? Isdeleted { get; set; }
        }
        public class UpdateStatus
        {
            public string Status { get; set; } = null!;
        }
    }
}
