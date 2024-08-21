using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Twitter;
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
    public async Task<IActionResult> FacebookResponse(string error = null, string error_code = null, string error_description = null, string error_reason = null, string state = null)
    {
        if (!string.IsNullOrEmpty(error))
        {
            var errorMessage = $"Facebook login error: {error_description ?? "Unknown error"}";

            ViewBag.ErrorMessage = errorMessage;
            return View("Error"); 
        }

        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded || result.Principal == null)
        {
            return RedirectToAction(nameof(LoginFacebook));
        }

        var claims = result.Principal.Identities
            .FirstOrDefault()?.Claims.Select(claim => new
            {
                claim.Type,
                claim.Value
            });
        return RedirectToAction("Index", "Home");
    }
    [HttpGet("signin-facebook")]
    public async Task<IActionResult> FacebookSignIn(string error = null, string error_code = null, string error_description = null, string error_reason = null, string state = null)
    {
        if (!string.IsNullOrEmpty(error))
        {
            var errorMessage = $"Facebook login error: {error_description ?? "Unknown error"}";

            ViewBag.ErrorMessage = errorMessage;
            return View("Error"); 
        }

        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded || result.Principal == null)
        {
            return RedirectToAction(nameof(LoginFacebook));
        }

        var claims = result.Principal.Identities
            .FirstOrDefault()?.Claims.Select(claim => new
            {
                claim.Type,
                claim.Value
            });

        return RedirectToAction("Index", "Home");
    }
    [HttpGet("login-twitter")]
    public IActionResult LoginTwitter()
    {
        var redirectUrl = Url.Action("TwitterResponse", "Auth");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, TwitterDefaults.AuthenticationScheme);
    }
 

    [HttpGet("twitter-response")]
    public async Task<IActionResult> TwitterResponse()
    
    {
        var state = HttpContext.Session.GetString("oauth_state");
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded || result?.Principal == null)
        {
            return RedirectToAction(nameof(LoginTwitter));
        }

        var claims = result.Principal.Identities
                             .FirstOrDefault()?.Claims.Select(claim => new
                             {
                                 claim.Type,
                                 claim.Value
                             }).ToList();

        return RedirectToAction("Index", "Home");
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
