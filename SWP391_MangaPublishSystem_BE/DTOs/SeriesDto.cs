using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public class SeriesDto
    {
        public int Seriesid { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public string Coverimageurl { get; set; } 
        public string Agerating { get; set; }
        public int Mangakaid { get; set; }
        public int? Tantoueditorid { get; set; }
        public string Publishformat { get; set; }
        public string Status { get; set; }
        public string Proposalfileurl { get; set; } 
        public DateTime? Createdat { get; set; }
        public DateTime? Approvedat { get; set; }
        public bool? Isdeleted { get; set; }

        public List<GenreSimpleDto> Genres { get; set; } = new List<GenreSimpleDto>();
        public List<TagSimpleDto> Tags { get; set; } = new List<TagSimpleDto>();

        public class Create
        {
            [Required(ErrorMessage = "Tiêu đề truyện không được để trống")]
            [MaxLength(255, ErrorMessage = "Tiêu đề truyện không được vượt quá 255 ký tự")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Mô tả truyện không được để trống")]
            public string Synopsis { get; set; }

            [Required(ErrorMessage = "Mã biên tập viên không được để trống")]
            public int Tantoueditorid { get; set; }

            [Required(ErrorMessage = "Vui lòng chọn giới hạn độ tuổi")]
            public string Agerating { get; set; }

            [Required(ErrorMessage = "Mã tác giả không được để trống")]
            public int Mangakaid { get; set; }

            public List<int> GenreIds { get; set; } = new List<int>();
            public List<int> TagIds { get; set; } = new List<int>();
        }

        public class Update
        {
            [MaxLength(255, ErrorMessage = "Tiêu đề truyện không được vượt quá 255 ký tự")]
            public string Title { get; set; }
            public string Synopsis { get; set; }
            public string Agerating { get; set; }

            public List<int> GenreIds { get; set; } = new List<int>();
            public List<int> TagIds { get; set; } = new List<int>();
        }

        public class UpdateStatus
        {
            [Required(ErrorMessage = "Trạng thái không được để trống")]
            public string Status { get; set; }
        }

        public class UpdatePublishFormat
        {
            [Required(ErrorMessage = "Định dạng xuất bản không được để trống")]
            public string Publishformat { get; set; }
        }

        public class GenreSimpleDto
        {
            public int GenreId { get; set; }
            public string GenreName { get; set; }
        }

        public class TagSimpleDto
        {
            public int TagId { get; set; }
            public string TagName { get; set; }
        }
    }
}
