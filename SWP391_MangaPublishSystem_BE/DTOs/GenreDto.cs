using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public class GenreDto
    {
        public int Genreid { get; set; }
        public string Genrename { get; set; } = null!;
        public string? Description { get; set; }

        public class Create
        {
            [Required(ErrorMessage = "Tên thể loại không được để trống")]
            [MaxLength(100, ErrorMessage = "Tên thể loại không được vượt quá 100 ký tự")]
            public string Genrename { get; set; } = null!;
            public string? Description { get; set; }
        }

        public class Update
        {
            [Required(ErrorMessage = "Tên thể loại không được để trống")]
            [MaxLength(100, ErrorMessage = "Tên thể loại không được vượt quá 100 ký tự")]
            public string Genrename { get; set; } = null!;
            public string? Description { get; set; }
        }
    }
}
