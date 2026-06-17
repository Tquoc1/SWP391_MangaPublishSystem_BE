using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class MangakaAssistantDto
    {
        public int ContractId { get; set; }
        public int MangakaId { get; set; }
        public int AssistantId { get; set; }
        public decimal SalaryAmount { get; set; }
        public string SalaryType { get; set; }
        public string ContractTerms { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? Createdat { get; set; }
        public bool? Isdeleted { get; set; }

        public class Create
        {
            public int MangakaId { get; set; }
            public int AssistantId { get; set; }
            public decimal SalaryAmount { get; set; }
            public string SalaryType { get; set; }
            public string ContractTerms { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public class Update
        {
            public decimal SalaryAmount { get; set; }
            public string SalaryType { get; set; }
            public string ContractTerms { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public class UpdateStatus
        {
            public string Status { get; set; }
        }
    }
}
