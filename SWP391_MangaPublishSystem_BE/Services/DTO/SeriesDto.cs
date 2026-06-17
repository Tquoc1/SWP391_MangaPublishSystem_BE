using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.DTO
{
    public class SeriesDto
    {

        public int Seriesid { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public string Coverimageurl { get; set; } 
        public string Agerating { get; set; }     //  ("G", "PG-13", "R-16", "R-18")
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
            public string Title { get; set; }
            public string Synopsis { get; set; }
            public string Agerating { get; set; }
            //public string Publishformat { get; set; }
            //public string Status { get; set; }
            //public bool? Isdeleted { get; set; }

            public List<int> GenreIds { get; set; } = new List<int>();
            public List<int> TagIds { get; set; } = new List<int>();
        }



        public class UpdateStatus
        {
            public string Status { get; set; }
        }

        public class UpdatePublishFormat
        {
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