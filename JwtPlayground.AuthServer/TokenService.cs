namespace JwtPlayground.AuthServer
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    public class TokenService
    {
        private readonly RsaSecurityKey _rsaKey;
        private readonly IConfiguration _config;
        public TokenService(RsaSecurityKey rsaKey, IConfiguration config)
        {
            _rsaKey = rsaKey;
            _config = config;
        }

        public TokenResponse GenerateToken(LoginRequest login)
        {
            var creds = new SigningCredentials(_rsaKey, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, login.Username) },
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            return new TokenResponse(
                new JwtSecurityTokenHandler().WriteToken(token),
                "Bearer");
        }

    }
}
