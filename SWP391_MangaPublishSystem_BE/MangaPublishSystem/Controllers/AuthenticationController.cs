using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Entities.Models;
using Services.DTO;

namespace MangaPublishSystem.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthenticationController(IConfiguration config, IAuthService authService, IUserService userService)
        {
            _config = config;
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDto.Login request)
        {
            var user = await _authService.GetUserByUsername(request.UserName);

            if (user == null)
            {
                return Unauthorized();
            }

            try
            {
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Passwordhash))
                {
                    return Unauthorized();
                }
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return Unauthorized();
            }

            var token = GenerateJSONWebToken(user);
            var refreshToken = await CreateRefreshToken(user);

            return Ok(new
            {
                userid = user.Userid,
                username = user.Username,
                fullname = user.Fullname,
                email = user.Email,
                roleid = user.Roleid,
                token,
                refreshToken
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthDto.Register request)
        {
            var existing = await _authService.GetUserByUsername(request.UserName);
            if (existing != null)
            {
                return Conflict("Username already exists.");
            }

            if (request.RoleId != 4 && request.RoleId != 5)
            {
                return BadRequest("RoleId must be 4 (Mangaka) or 5 (Assistant).");
            }

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return BadRequest("Fullname is required.");
            }

            var user = new User
            {
                Username = request.UserName,
                Passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Fullname = request.FullName,
                Email = request.Email,
                Roleid = request.RoleId,
                Createdat = DateTime.Now,
                Isdeleted = false
            };

            var result = await _authService.CreateUser(user);
            if (result <= 0)
            {
                return BadRequest();
            }

            //if (user.Roleid == 4)
            //{
            //    await _userService.AddMangakaProfile(new MangakaProfile { Userid = user.Userid });
            //}
            //else if (user.Roleid == 5)
            //{
            //    await _userService.AddAssistantProfile(new AssistantProfile { Userid = user.Userid });
            //}

            var token = GenerateJSONWebToken(user);
            var refreshToken = await CreateRefreshToken(user);

            return Ok(new { user.Userid, user.Username, user.Roleid, token, refreshToken });
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] AuthDto.RefreshToken request)
        {
            var storedToken = await _authService.GetUserToken(request.Token);
            if (storedToken == null || storedToken.Isrevoked == true || storedToken.Expiresat <= DateTime.Now)
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserById(storedToken.Userid);
            if (user == null)
            {
                return Unauthorized();
            }

            await _authService.RevokeUserToken(storedToken);

            var token = GenerateJSONWebToken(user);
            var refreshToken = await CreateRefreshToken(user);

            return Ok(new { token, refreshToken });
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromBody] AuthDto.RefreshToken request)
        {
            var storedToken = await _authService.GetUserToken(request.Token);
            if (storedToken == null)
            {
                return Ok();
            }

            await _authService.RevokeUserToken(storedToken);
            return Ok();
        }

        private string GenerateJSONWebToken(User systemUserAccount)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"]
                    , _config["Jwt:Audience"]
                    , new Claim[]
                    {
                        new(ClaimTypes.Name, systemUserAccount.Username),
                        //new(ClaimTypes.Email, systemUserAccount.Email),
                        new(ClaimTypes.Role, systemUserAccount.Roleid.ToString()),
                        new("roleid", systemUserAccount.Roleid.ToString()),
                        new("userid", systemUserAccount.Userid.ToString())
                    },
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
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

            await _authService.CreateUserToken(tokenEntity);
            return refreshToken;
        }


        [HttpPost("create-staff")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> CreateStaff([FromBody] AuthDto.CreateStaffRequest request)
        {
            var existing = await _authService.GetUserByUsername(request.UserName);
            if (existing != null)
            {
                return Conflict("Username already exists.");
            }

            if (request.RoleId == 4 || request.RoleId == 5)
            {
                return BadRequest("This API is only for creating staff accounts (Admin, Editor, Web Admin).");
            }

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return BadRequest("Fullname is required.");
            }

 
            var user = new User
            {
                Username = request.UserName,
                Passwordhash = BCrypt.Net.BCrypt.HashPassword(request.Password), 
                Fullname = request.FullName,
                Email = request.Email,
                Roleid = request.RoleId,
                Createdat = DateTime.Now,
                Isdeleted = false
            };

            var result = await _authService.CreateUser(user);
            if (result <= 0)
            {
                return BadRequest("Failed to create staff account.");
            }

            var token = GenerateJSONWebToken(user);
            var refreshToken = await CreateRefreshToken(user);

            return Ok(new
            {
                Message = "Staff account created successfully by Admin",
                user.Userid,
                user.Username,
                user.Roleid,
                token,
                refreshToken
            });
        }
    }
}

