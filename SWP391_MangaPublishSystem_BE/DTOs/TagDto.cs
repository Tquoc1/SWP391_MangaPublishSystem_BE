using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public class TagDto
    {
        public int Tagid { get; set; }
        public string Tagname { get; set; } = null!;

        public class Create
        {
            [Required]
            [MaxLength(100)]
            public string Tagname { get; set; } = null!;
        }

        public class Update
        {
            [Required]
            [MaxLength(100)]
            public string Tagname { get; set; } = null!;
        }
    }
}
