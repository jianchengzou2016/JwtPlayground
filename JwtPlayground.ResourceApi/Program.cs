using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var authServerbase = builder.Configuration["AuthServer:BaseUrl"] ?? throw new InvalidOperationException("appsettings.json missing definition for the AuthServer");

var httpHandler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator }; //TODO: can I use this in prod code? what's the risk?
using var http = new HttpClient(httpHandler);
var json = http.GetStringAsync($"{authServerbase}/publickey").GetAwaiter().GetResult();

using var doc = System.Text.Json.JsonDocument.Parse(json);
var pem = doc.RootElement.GetProperty("key").GetString();

var rsa = RSA.Create();
rsa.ImportFromPem(pem);
var rsakey = new RsaSecurityKey(rsa);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = rsakey,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents { 
            OnAuthenticationFailed = context => {
                Console.WriteLine("Auth failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated. Claims:");
                foreach (var claim in context.Principal.Claims)
                    Console.WriteLine($" {claim.Type}:{claim.Value}");
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization(); //TODO: why adding Authorization not Authentication?

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();//TODO: I don't get why calling it at the app level again?
app.UseAuthorization();

app.MapControllers();

app.Run();
