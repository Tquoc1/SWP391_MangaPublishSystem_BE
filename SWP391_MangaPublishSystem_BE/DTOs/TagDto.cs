using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public class TagDto
    {
        public int Tagid { get; set; }
        public string Tagname { get; set; } = null!;

        public class Create
        {
            [Required(ErrorMessage = "Tên thẻ không được để trống")]
            [MaxLength(100, ErrorMessage = "Tên thẻ không được vượt quá 100 ký tự")]
            public string Tagname { get; set; } = null!;
        }

        public class Update
        {
            [Required(ErrorMessage = "Tên thẻ không được để trống")]
            [MaxLength(100, ErrorMessage = "Tên thẻ không được vượt quá 100 ký tự")]
            public string Tagname { get; set; } = null!;
        }
    }
}
