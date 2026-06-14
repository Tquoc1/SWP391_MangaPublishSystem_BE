using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace Services.DTO
{
    public class SeriesDto
    {
        public int Seriesid { get; set; }
        public string Title { get; set; }
        public string Synopsis { get; set; }
        public int Mangakaid { get; set; }
        public int Tantoueditorid { get; set; }
        public string Publishformat { get; set; }
        public string Status { get; set; }
        public string Proposalfileurl { get; set; }
        public DateTime? Createdat { get; set; }
        public DateTime? Approvedat { get; set; }
        public bool? Isdeleted { get; set; }

        public class Create
        {
            public string Title { get; set; }
            public string Synopsis { get; set; }
            public int Mangakaid { get; set; }
            public int Tantoueditorid { get; set; }
        }

        public class Update
        {

            public string Title { get; set; }
            public string Synopsis { get; set; }
            public string Publishformat { get; set; }
            public string Status { get; set; }
            public bool? Isdeleted { get; set; }
        }
        public class UpdateStatus
        {
            public string Status { get; set; }
        }
        public class UpdatePublishFormat
        {
            public string Publishformat { get; set; }
        }
    }
}
