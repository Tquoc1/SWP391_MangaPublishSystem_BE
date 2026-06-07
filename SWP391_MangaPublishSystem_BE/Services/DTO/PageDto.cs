
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
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
            //public int Pageid { get; set; }

            public int Pagenumber { get; set; }
            public string Status { get; set; }
            public bool? Isdeleted { get; set; }
        }
    }

    
}
