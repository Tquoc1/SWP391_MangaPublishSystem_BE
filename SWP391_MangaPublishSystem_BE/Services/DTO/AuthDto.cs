namespace Services.DTO
{
    public class AuthDto
    {
        public sealed record Login(string UserName, string Password);

        public sealed record Register(
            string UserName,
            string Password,
            string FullName,
            string Email,
            int RoleId,
            string PenName,
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

        public sealed record RefreshToken(string Token);
    }
}