using System;
using System.Security.Claims;

namespace Kuvio.Kernel.Auth
{
    public static class B2CPrincipalExtensions
    {
        public static string DisplayName(this ClaimsPrincipal principal) => principal.Get("name") ?? principal.Get("http://schemas.microsoft.com/identity/claims/givenname") ?? principal.Get(ClaimTypes.Name);
        public static string Email(this ClaimsPrincipal principal) => principal.Get(ClaimTypes.Email);
        public static string AzureID(this ClaimsPrincipal principal) => principal.Get("http://schemas.microsoft.com/identity/claims/objectidentifier");
        public static int ID(this ClaimsPrincipal principal) => int.Parse(principal.Get(ClaimTypes.NameIdentifier));
        public static string Get(this ClaimsPrincipal principal, string claimName) => principal.FindFirst(claimName)?.Value;
    }
}
