using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace PhotosOfUs.Server
{
    [Authorize]
    public class SessionController : Controller
    {
        [HttpGet("/Session/SignIn")]
        [AllowAnonymous]
        public IActionResult SignIn(string redirectUrl = null)
        {
            if (User?.Identity?.IsAuthenticated == true) return Redirect($"{CurrentUrl}/Admin/Dashboard");
            if (redirectUrl == null) redirectUrl = $"{CurrentUrl}/Admin/Dashboard";

            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ResetPassword()
        //{
        //    var redirectUrl = Url.Page("/Admin/Dashboard");
        //    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        //    properties.Items[AzureAdB2COptions.PolicyAuthenticationProperty] = AzureAdB2COptions.ResetPasswordPolicyId;
        //    return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        //}

        //[HttpGet]
        //public IActionResult EditProfile()
        //{
        //    var redirectUrl = Url.Page("/Admin/Dashboard");
        //    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        //    properties.Items[AzureAdB2COptions.PolicyAuthenticationProperty] = AzureAdB2COptions.EditProfilePolicyId;
        //    return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        //}

        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = $"{CurrentUrl}/Home/Index";
            return SignOut(new AuthenticationProperties { RedirectUri = callbackUrl },
                "B2C", OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignedOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToPage("/Home/Index");
            }

            return View();
        }

        public string CurrentUrl => $"{Request.Scheme}{Uri.SchemeDelimiter}{Request.Host.Value}";
    }
}
