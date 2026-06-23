using System.ComponentModel.DataAnnotations;

namespace Services.DTO
{
    public class UserDto
    {
        public sealed record UpdateMangakaProfile(
            [Required(ErrorMessage = "Họ và tên không được để trống")] string FullName,
            string? PenName,
            string? Bio,
            [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ")] string? PhoneNumber,
            string? BankName,
            string? BankAccountNumber,
            string? BankAccountName
        );

        public sealed record UpdateAssistantProfile(
            [Required(ErrorMessage = "Họ và tên không được để trống")] string FullName,
            string? PortfolioUrl,
            [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ")] string? PhoneNumber,
            bool? IsAvailable,
            string? Skills,
            string? SoftwareUsed,
            string? BankName,
            string? BankAccountNumber,
            string? BankAccountName
        );

        public sealed record AvailableAssistant(
            int UserId,
            string Username,
            string FullName,
            string Email,
            string? AvatarUrl,
            string? PortfolioUrl,
            string? Skills,
            string? SoftwareUsed
        );

        public sealed record AdminUserResponse(
            int UserId,
            string Username,
            string FullName,
            string Email,
            int RoleId,
            string RoleName,
            string Status,
            DateTime? CreatedAt
        );

        public class AdminCreateUserRequest
        {
            [Required(ErrorMessage = "Username không được để trống")]
            public string Username { get; set; }
            [Required(ErrorMessage = "Mật khẩu không được để trống")]
            public string Password { get; set; }
            [Required(ErrorMessage = "Họ và tên không được để trống")]
            public string FullName { get; set; }
            public string? Email { get; set; }
            [Required(ErrorMessage = "Quyền (Role) không được để trống")]
            public int RoleId { get; set; }
        }

        public class AdminUpdateUserRequest
        {
            [Required(ErrorMessage = "Họ và tên không được để trống")]
            public string FullName { get; set; }
            public string? Email { get; set; }
            [Required(ErrorMessage = "Quyền (Role) không được để trống")]
            public int RoleId { get; set; }
            public string? Password { get; set; }
        }

        public class AdminUpdateStatusRequest
        {
            [Required(ErrorMessage = "Trạng thái không được để trống")]
            [AllowedValues("Active", "Inactive", "Locked", ErrorMessage = "Trạng thái không hợp lệ")]
            public string Status { get; set; }
        }
    }
}