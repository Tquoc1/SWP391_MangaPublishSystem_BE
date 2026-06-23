using System;
using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public class PageLayerDto
    {
        public int Layerid { get; set; }
        public int Pageid { get; set; }
        public int Uploaderid { get; set; }
        public string Layername { get; set; }
        public string Fileurl { get; set; }
        public int? Zindex { get; set; }
        public decimal Opacity { get; set; }
        public int? Versionnumber { get; set; }
        public bool? Isvisible { get; set; }
        public bool Isdeleted { get; set; }
        public DateTime? Createdat { get; set; }

        public class Create
        {
            [Required(ErrorMessage = "Page ID không được để trống")]
            public int Pageid { get; set; }

            [Required(ErrorMessage = "Người tải lên không được để trống")]
            public int Uploaderid { get; set; }

            [Required(ErrorMessage = "Tên layer không được để trống")]
            [MaxLength(255, ErrorMessage = "Tên layer không được vượt quá 255 ký tự")]
            public string Layername { get; set; }
            public int? Zindex { get; set; }
            public decimal? Opacity { get; set; }
        }

        public class Update
        {
            [MaxLength(255, ErrorMessage = "Tên layer không được vượt quá 255 ký tự")]
            public string Layername { get; set; }
            public int? Zindex { get; set; }
            public decimal? Opacity { get; set; }
            public int? Versionnumber { get; set; }
        }
    }
}
