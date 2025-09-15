using JwtPlayground.AuthServer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rsa = RSA.Create(2048);
var kid = Guid.NewGuid().ToString();
var rsaKey = new RsaSecurityKey(rsa) { KeyId = kid };
builder.Services.AddSingleton(rsaKey);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapGet("/.well-known/jwks.json", () => {
    var parameters = rsa.ExportParameters(false);

    var jwk = new JsonWebKey
    {
        Kty = "RSA",
        Use = "sig",
        Kid = kid,  // key ID (must match token header `kid`)
        Alg = "RS256",
        N = Base64UrlEncoder.Encode(parameters.Modulus),
        E = Base64UrlEncoder.Encode(parameters.Exponent)
    };

    var jwks = new { keys = new[] { jwk } };
    return Results.Json(jwks);
});

app.Run();
