using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using igovit.Models;
using System.Net;

[Route("api")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("Login")]
    public async Task<HttpResponseMessage> Login([FromBody] LoginModel model)
    {
        System.Console.WriteLine("passou aqui");
        // todo #1
        if (model.Username == "foo" && model.Password == "bar")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    [HttpGet("IsAuthenticated")]
    public IActionResult IsAuthenticated()
    {
        return Ok(User.Identity?.IsAuthenticated);
    }
}