﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using PhotosOfUs.Model.Models;
using Kuvio.Kernel.Auth;
using Microsoft.AspNetCore.Authorization;

namespace PhotosOfUs.Web.Controllers
{
    [Area("Users")]
    public class SessionController : Controller
    {
        private PhotosOfUsContext _context;

        public SessionController(IOptions<AzureAdB2COptions> b2cOptions, PhotosOfUsContext context)
        {
            AzureAdB2COptions = b2cOptions.Value;
            _context = context;
        }

        public AzureAdB2COptions AzureAdB2COptions { get; set; }

        [HttpGet]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Page("Photographer/Index");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectDefaults.AuthenticationScheme);     
        }

        [HttpGet]
        public IActionResult SignInPhotographer()
        {
            var redirectUrl = Url.Page("Photographer/Index");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items[AzureAdB2COptions.PolicyAuthenticationProperty] = AzureAdB2COptions.SignUpSignInPolicyIdPhotographer;
            return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            var redirectUrl = Url.Page("Photographer/Index");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items[AzureAdB2COptions.PolicyAuthenticationProperty] = AzureAdB2COptions.ResetPasswordPolicyId;
            return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditProfile()
        {
            var redirectUrl = Url.Page("Photographer/Index");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items[AzureAdB2COptions.PolicyAuthenticationProperty] = AzureAdB2COptions.EditProfilePolicyId;
            return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Authorize]
        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Page("Home/Index");
            return SignOut(new AuthenticationProperties { RedirectUri = callbackUrl },
                "B2C", OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult SignedOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToPage("Home/Index");
            }

            return View();
        }
    }
}
