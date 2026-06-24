using System;
using System.ComponentModel.DataAnnotations;

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
        public string MangakaName { get; set; }
        public string AssistantName { get; set; }
        public string? ContractFileUrl { get; set; }

        public class Create
        {
            [Required(ErrorMessage = "Mangaka ID không được để trống")]
            public int MangakaId { get; set; }

            [Required(ErrorMessage = "Assistant ID không được để trống")]
            public int AssistantId { get; set; }

            [Required(ErrorMessage = "Lương không được để trống")]
            [Range(0, double.MaxValue, ErrorMessage = "Lương không được âm")]
            public decimal SalaryAmount { get; set; }

            [Required(ErrorMessage = "Loại lương không được để trống")]
            [AllowedValues("Fixed", "PerChapter", "Monthly", ErrorMessage = "Loại lương không hợp lệ")]
            public string SalaryType { get; set; }

            [Required(ErrorMessage = "Điều khoản hợp đồng không được để trống")]
            public string ContractTerms { get; set; }

            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? ContractFileUrl { get; set; }
        }

        public class Update
        {
            [Range(0, double.MaxValue, ErrorMessage = "Lương không được âm")]
            public decimal SalaryAmount { get; set; }
            [AllowedValues("Fixed", "PerChapter", "Monthly", ErrorMessage = "Loại lương không hợp lệ")]
            public string SalaryType { get; set; }
            public string ContractTerms { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? ContractFileUrl { get; set; }
        }

        public class UpdateStatus
        {
            [Required(ErrorMessage = "Trạng thái không được để trống")]
            [AllowedValues("Terminated", "Active", "Pending", "Expired", "Suspended", "Completed", ErrorMessage = "Trạng thái không hợp lệ")]
            public string Status { get; set; }
        }
    }
}
