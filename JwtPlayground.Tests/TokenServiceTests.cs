using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using JwtPlayground.AuthServer;
using JwtPlayground.AuthServer.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace JwtPlayground.Tests;

public class TokenServiceTests
{
    private static readonly JwtSecurityTokenHandler Handler = new();

    [Fact]
    public void GenerateToken_ReturnsBearerTokenWithConfiguredIssuerAudienceAndSubject()
    {
        using var rsa = RSA.Create(2048);
        var rsaKey = new RsaSecurityKey(rsa) { KeyId = "test-key" };
        var tokenService = new TokenService(rsaKey, BuildConfiguration());

        var response = tokenService.GenerateToken(new LoginRequest("alice", "123"));
        var jwt = Handler.ReadJwtToken(response.access_token);

        Assert.Equal("Bearer", response.token_type);
        Assert.Equal("JwtPlayground.AuthServer", jwt.Issuer);
        Assert.Equal("JwtPlayground.ResourceApi", Assert.Single(jwt.Audiences));
        Assert.Equal("alice", jwt.Subject);
        Assert.True(jwt.ValidTo > DateTime.UtcNow.AddHours(23));
    }

    [Fact]
    public void ExportPublicKeyPem_ReturnsValidPemThatMatchesSourceRsa()
    {
        using var rsa = RSA.Create(2048);

        var pem = KeysController.ExportPublicKeyPem(rsa);

        Assert.Contains("BEGIN PUBLIC KEY", pem);
        Assert.Contains("END PUBLIC KEY", pem);

        using var imported = RSA.Create();
        imported.ImportFromPem(pem);

        Assert.Equal(
            rsa.ExportSubjectPublicKeyInfo(),
            imported.ExportSubjectPublicKeyInfo());
    }

    private static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "JwtPlayground.AuthServer",
                ["Jwt:Audience"] = "JwtPlayground.ResourceApi"
            })
            .Build();
}
