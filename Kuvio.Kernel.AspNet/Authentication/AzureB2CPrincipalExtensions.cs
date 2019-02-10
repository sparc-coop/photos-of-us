using System.Security.Claims;

namespace Kuvio.Kernel.AspNet
{
    public static class AzureB2CPrincipalExtensions
    {
        public static string DisplayName(this ClaimsPrincipal principal) => principal.Get("name") ?? principal.Get("http://schemas.microsoft.com/identity/claims/givenname") ?? principal.Get(ClaimTypes.Name);

        public static string Email(this ClaimsPrincipal principal) => principal.Get(ClaimTypes.Email) ?? principal.Get("emails");

        public static string AzureID(this ClaimsPrincipal principal) => principal.Get("http://schemas.microsoft.com/identity/claims/objectidentifier");

        public static int ID(this ClaimsPrincipal principal) {
            var id = principal.FindFirst(x => x.Type == "ID")?.Value;
            return id == null ? -1 : int.Parse(id);
        }
        
        public static string Get(this ClaimsPrincipal principal, string claimName) => principal.FindFirst(claimName)?.Value;
    }
}
