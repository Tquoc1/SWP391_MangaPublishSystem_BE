using System;
using System.ComponentModel.DataAnnotations;

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
            [Required(ErrorMessage = "Series ID không được để trống")]
            public int Seriesid { get; set; }

            [Required(ErrorMessage = "Số chương không được để trống")]
            [Range(1, int.MaxValue, ErrorMessage = "Số chương phải lớn hơn 0")]
            public int Chapternumber { get; set; }

            [Required(ErrorMessage = "Tiêu đề chương không được để trống")]
            [MaxLength(255, ErrorMessage = "Tiêu đề chương không được vượt quá 255 ký tự")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Hạn chót không được để trống")]
            public DateTime Deadline { get; set; }
        }

        public class Update
        {
            [System.Text.Json.Serialization.JsonIgnore]
            public int Chapterid { get; set; }

            [Required(ErrorMessage = "Số chương không được để trống")]
            [Range(1, int.MaxValue, ErrorMessage = "Số chương phải lớn hơn 0")]
            public int Chapternumber { get; set; }

            [Required(ErrorMessage = "Tiêu đề chương không được để trống")]
            [MaxLength(255, ErrorMessage = "Tiêu đề chương không được vượt quá 255 ký tự")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Hạn chót không được để trống")]
            public DateTime Deadline { get; set; }
            //public string Status { get; set; }
            //public bool? Isdeleted { get; set; }
        }
    }
}
