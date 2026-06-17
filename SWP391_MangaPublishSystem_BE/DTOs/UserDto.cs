namespace Services.DTO
{
    public class UserDto
    {
        public sealed record UpdateProfile(
            string FullName,
            string? PenName,
            string? Bio,
            string? PhoneNumber,
            string? BankName,
            string? BankAccountNumber,
            string? BankAccountName,
            string? PortfolioUrl,
            bool? IsAvailable,
            string? Skills,
            string? SoftwareUsed
        );
    }
}