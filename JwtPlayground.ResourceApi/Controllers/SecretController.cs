namespace JwtPlayground.ResourceApi.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http.HttpResults;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class SecretController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var user = HttpContext.User;
            Console.WriteLine(user.Identity?.IsAuthenticated);
            foreach(var claim in user.Claims)
                Console.WriteLine($" {claim.Type} = {claim.Value}");

            return Ok(new { message = "Secret data (validated by RSA public key)", time = DateTime.UtcNow, name = "jim" });
        }
    }
}
