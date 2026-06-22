using System;
using System.ComponentModel.DataAnnotations;

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
            [Required(ErrorMessage = "Chapter ID không được để trống")]
            public int Chapterid { get; set; }

            [Required(ErrorMessage = "Số trang không được để trống")]
            [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0")]
            public int Pagenumber { get; set; }
        }

        public class Update
        {
            [Required(ErrorMessage = "Số trang không được để trống")]
            [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0")]
            public int Pagenumber { get; set; }
        }
    }
}
