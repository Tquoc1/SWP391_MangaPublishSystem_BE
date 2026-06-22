using System.ComponentModel.DataAnnotations;

namespace Services.DTO
{
    public class AuthDto
    {
        public sealed record Login(
            [Required(ErrorMessage = "Tên đăng nhập không được để trống")] string UserName, 
            [Required(ErrorMessage = "Mật khẩu không được để trống")] string Password
        );

        public sealed record Register(
            [Required(ErrorMessage = "Tên đăng nhập không được để trống")] string UserName,
            [Required(ErrorMessage = "Mật khẩu không được để trống")] 
            [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")] string Password,
            [Required(ErrorMessage = "Họ và tên không được để trống")] string FullName,
            [Required(ErrorMessage = "Email không được để trống")] 
            [EmailAddress(ErrorMessage = "Email không đúng định dạng")] string Email,
            [Required(ErrorMessage = "Vai trò không được để trống")] int RoleId
        );

        public sealed record RefreshToken(
            [Required(ErrorMessage = "Token không được để trống")] string Token
        );

        public class CreateStaffRequest
        {
            [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Mật khẩu không được để trống")]
            [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Họ và tên không được để trống")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Email không được để trống")]
            [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Vai trò không được để trống")]
            public int RoleId { get; set; }
        }
    }
}