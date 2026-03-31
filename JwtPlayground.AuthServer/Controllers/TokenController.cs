using Microsoft.AspNetCore.Mvc;


namespace JwtPlayground.AuthServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public TokenController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        public IActionResult GetToken([FromBody] LoginRequest login)
        {
            if (string.IsNullOrWhiteSpace(login.Username))
            {
                return BadRequest();
            }

            return Ok(_tokenService.GenerateToken(login));
        }

    }
}
