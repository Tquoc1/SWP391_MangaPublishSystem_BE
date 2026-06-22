using Entities.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Repository;
using Services.Constants;
using Services.DTO;
using Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Implement
{
    public class AuthService : IAuthService
    {
        private readonly AuthRepository _authRepository;
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public AuthService(AuthRepository authRepository, IConfiguration config, IUserService userService)
        {
            _authRepository = authRepository;
            _config = config;
            _userService = userService;
        }

        public async Task<AuthDto.AuthResponse> LoginAsync(AuthDto.Login request)
        {
            var user = await _authRepository.GetUserByUsername(request.UserName);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không chính xác.");
            }

            if (user.Status != "Active")
            {
                throw new UnauthorizedAccessException("Tài khoản của bạn đã bị khóa hoặc chưa kích hoạt.");
            }

            try
            {
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Passwordhash))
                {
                    throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không chính xác.");
                }
            }
            catch (BCrypt.Net.SaltParseException)
            {
                throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không chính xác.");
            }

            var token = GenerateJSONWebToken(user);
            var refreshToken = await CreateRefreshToken(user);

            return new AuthDto.AuthResponse(
                user.Userid,
                user.Username,
                user.Fullname,
                user.Email,
                user.Roleid,
                token,
                refreshToken
            );
        }

        public async Task<AuthDto.AuthResponse> RegisterAsync(AuthDto.Register request)
        {
            var existing = await _authRepository.GetUserByUsername(request.UserName);
            if (existing != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            if (request.RoleId != 4 && request.RoleId != 5)
            {
                throw new ArgumentException("RoleId must be 4 (Mangaka) or 5 (Assistant).");
            }

            var user = new User
            {
                Username = request.UserName,
                Passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Fullname = request.FullName,
                Email = request.Email,
                Roleid = request.RoleId,
                Createdat = DateTime.Now,
                Isdeleted = false,
                Status = "Active"
            };

            await _authRepository.CreateAsync(user);

            if (user.Roleid == 4)
            {
                await _userService.AddMangakaProfile(new MangakaProfile
                {
                    Userid = user.Userid,
                    PenName = request.FullName
                });
            }
            else if (user.Roleid == 5)
            {
                await _userService.AddAssistantProfile(new AssistantProfile { Userid = user.Userid });
            }

            var token = GenerateJSONWebToken(user);
            var refreshToken = await CreateRefreshToken(user);

            return new AuthDto.AuthResponse(
                user.Userid,
                user.Username,
                user.Fullname,
                user.Email,
                user.Roleid,
                token,
                refreshToken
            );
        }

        public async Task<AuthDto.AuthResponse> RefreshTokenAsync(AuthDto.RefreshToken request)
        {
            var storedToken = await _authRepository.GetUserToken(request.Token);
            if (storedToken == null || storedToken.Isrevoked == true || storedToken.Expiresat <= DateTime.Now)
            {
                throw new UnauthorizedAccessException("Token is invalid or expired.");
            }

            var user = await _userService.GetUserById(storedToken.Userid);
            if (user == null || user.Status != "Active")
            {
                throw new UnauthorizedAccessException("User is invalid or inactive.");
            }

            storedToken.Isrevoked = true;
            await _authRepository.UpdateUserToken(storedToken);

            var token = GenerateJSONWebToken(user);
            var refreshToken = await CreateRefreshToken(user);

            return new AuthDto.AuthResponse(
                user.Userid,
                user.Username,
                user.Fullname,
                user.Email,
                user.Roleid,
                token,
                refreshToken
            );
        }

        public async Task<AuthDto.AuthResponse> CreateStaffAsync(AuthDto.CreateStaffRequest request)
        {
            var existing = await _authRepository.GetUserByUsername(request.UserName);
            if (existing != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            if (request.RoleId == 4 || request.RoleId == 5)
            {
                throw new ArgumentException("This API is only for creating staff accounts.");
            }

            var user = new User
            {
                Username = request.UserName,
                Passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Fullname = request.FullName,
                Email = request.Email,
                Roleid = request.RoleId,
                Createdat = DateTime.Now,
                Isdeleted = false,
                Status = "Active"
            };

            await _authRepository.CreateAsync(user);

            var token = GenerateJSONWebToken(user);
            var refreshToken = await CreateRefreshToken(user);

            return new AuthDto.AuthResponse(
                user.Userid,
                user.Username,
                user.Fullname,
                user.Email,
                user.Roleid,
                token,
                refreshToken
            );
        }

        public async Task LogoutAsync(AuthDto.RefreshToken request)
        {
            var storedToken = await _authRepository.GetUserToken(request.Token);
            if (storedToken != null)
            {
                storedToken.Isrevoked = true;
                await _authRepository.UpdateUserToken(storedToken);
            }
        }

        public Task<User> GetUserByUsername(string userName)
        {
            return _authRepository.GetUserByUsername(userName);
        }

        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, ((UserRole)user.Roleid).ToString()),
                new Claim("roleid", user.Roleid.ToString()),
                new Claim("userid", user.Userid.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> CreateRefreshToken(User user)
        {
            var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var tokenEntity = new UserToken
            {
                Userid = user.Userid,
                Token = refreshToken,
                Isrevoked = false,
                Expiresat = DateTime.Now.AddDays(7)
            };

            await _authRepository.CreateUserToken(tokenEntity);
            return refreshToken;
        }
    }
}