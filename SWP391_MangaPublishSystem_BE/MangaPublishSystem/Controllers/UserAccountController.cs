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

namespace MangaPublishSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserAccountController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserAccountService _userAccountsService;

        public UserAccountController(IConfiguration config, IUserAccountService userAccountsService)
        {
            _config = config;
            _userAccountsService = userAccountsService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userAccountsService.GetUserAccount(request.UserName, request.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var token = GenerateJSONWebToken(user);

            return Ok(token);
        }

        private string GenerateJSONWebToken(User systemUserAccount)
        {
            if (systemUserAccount.Roleid == 4 || systemUserAccount.Roleid == 5)
            {
                return string.Empty;
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"]
                    , _config["Jwt:Audience"]
                    , new Claim[]
                    {
                        new(ClaimTypes.Name, systemUserAccount.Username),
                        //new(ClaimTypes.Email, systemUserAccount.Email),
                        new(ClaimTypes.Role, systemUserAccount.Roleid.ToString()),
                    },
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        public sealed record LoginRequest(string UserName, string Password);
    }
}
