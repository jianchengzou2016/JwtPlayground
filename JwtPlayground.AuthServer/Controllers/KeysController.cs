namespace JwtPlayground.AuthServer.Controllers
{
    using System.Security.Cryptography;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using System.Text;

    [ApiController]
    public class KeysController : ControllerBase
    {
        private readonly RsaSecurityKey _rsaKey;

        public KeysController(RsaSecurityKey rsaKey)
        {
            _rsaKey = rsaKey;
        }

        [HttpGet("/publickey")]
        public IActionResult GetPublicKey()
        {
            return Ok(new { key = ExportPublicKeyPem(_rsaKey.Rsa), kid = _rsaKey.KeyId });
        }

        public static string ExportPublicKeyPem(RSA rsa)
        {
            var publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
            var base64 = Convert.ToBase64String(publicKeyBytes);
            var sb = new StringBuilder();
            sb.AppendLine("-----BEGIN PUBLIC KEY-----");
            for (var i = 0; i < base64.Length; i += 64)
            {
                sb.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));
            }

            sb.AppendLine("-----END PUBLIC KEY-----");
            return sb.ToString();
        }
    }
}
