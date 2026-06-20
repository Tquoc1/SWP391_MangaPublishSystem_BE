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
            [Required]
            [MaxLength(100)]
            public string Genrename { get; set; } = null!;
            public string? Description { get; set; }
        }

        public class Update
        {
            [Required]
            [MaxLength(100)]
            public string Genrename { get; set; } = null!;
            public string? Description { get; set; }
        }
    }
}
