using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JwtPlayground.AuthServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController:ControllerBase
    {
        private readonly RsaSecurityKey _rsaKey;
        private readonly IConfiguration _config;
        public TokenController(RsaSecurityKey rsaKey, IConfiguration config)
        {
            _rsaKey = rsaKey;
            _config = config;
        }

        [HttpPost]
        public IActionResult GetToken([FromBody] LoginRequest login)
        {
            if (string.IsNullOrWhiteSpace(login.Username)) return BadRequest();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, login.Username),
            };
            var creds = new SigningCredentials(_rsaKey, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            return Ok(new {access_token = new JwtSecurityTokenHandler().WriteToken(token), token_type = "Bearer"});
        }

        [HttpGet]
        public string GenerateToken([FromBody] LoginRequest login)
        {
            var creds = new SigningCredentials(_rsaKey, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: new[] { new Claim(ClaimTypes.NameIdentifier, login.Username) },
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
