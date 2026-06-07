using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    // 1. DTO dùng cho hàm GET (Trả dữ liệu về)
    public class PageLayerDto
    {
        public int Layerid { get; set; }
        public int Pageid { get; set; }
        public int Uploaderid { get; set; }
        public string Layername { get; set; }
        public string Fileurl { get; set; }
        public int? Zindex { get; set; }
        public int? Versionnumber { get; set; }
        public bool? Isvisible { get; set; }
        public DateTime? Createdat { get; set; }

        public class Create
        {
            public int Pageid { get; set; }
            public int Uploaderid { get; set; }
            public string Layername { get; set; }
            public int? Zindex { get; set; }
        }

        public class Update
        {
            public string Layername { get; set; }
            public int? Zindex { get; set; }
            public int? Versionnumber { get; set; }
            public bool? Isvisible { get; set; }
        }
    }
}
