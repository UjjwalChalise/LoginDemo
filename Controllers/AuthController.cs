using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    [HttpGet("login-facebook")]
    public IActionResult LoginFacebook()
    {
        var redirectUrl = Url.Action("FacebookResponse", "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, FacebookDefaults.AuthenticationScheme);
    }

    [HttpGet("facebook-response")]
    public async Task<IActionResult> FacebookResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (result?.Principal == null)
            return RedirectToAction(nameof(LoginFacebook));

        var claims = result.Principal.Identities
                        .FirstOrDefault()?.Claims.Select(claim => new
                        {
                            claim.Type,
                            claim.Value
                        });

        return Json(claims);
    }

    [Authorize]
    public IActionResult Profile()
    {
        return View(User.Claims.Select(claim => new { claim.Type, claim.Value }));
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}
