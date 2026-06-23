using Services.DTO;

namespace Services.Interface
{
    public interface IAuthService
    {
        Task<AuthDto.AuthResponse> LoginAsync(AuthDto.Login request);
        Task<AuthDto.AuthResponse> RegisterAsync(AuthDto.Register request);
        Task<AuthDto.AuthResponse> RefreshTokenAsync(AuthDto.RefreshToken request);
        Task<AuthDto.AuthResponse> CreateStaffAsync(AuthDto.CreateStaffRequest request);
        Task LogoutAsync(AuthDto.RefreshToken request);
        
        // Cần thiết nếu UserService dùng
        Task<Entities.Models.User> GetUserByUsername(string userName);
    }
}