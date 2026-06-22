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
    }
}