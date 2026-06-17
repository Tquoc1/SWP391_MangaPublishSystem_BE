namespace Services.DTO
{
    public class UserDto
    {
        public sealed record UpdateMangakaProfile(
            string FullName,
            string? PenName,
            string? Bio,
            string? PhoneNumber,
            string? BankName,
            string? BankAccountNumber,
            string? BankAccountName,
            string? AvatarUrl
        );

        public sealed record UpdateAssistantProfile(
            string FullName,
            string? AvatarUrl,
            string? PortfolioUrl,
            string? PhoneNumber,
            bool? IsAvailable,
            string? Skills,
            string? SoftwareUsed,
            string? BankName,
            string? BankAccountNumber,
            string? BankAccountName
        );
    }
}