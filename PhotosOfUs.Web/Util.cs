using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhotosOfUs.Web
{
    public static class Util
    {
        public static int ID(this ClaimsPrincipal principal)
        {
            var id = principal.FindFirst(x => x.Type == "ID")?.Value;
            return id == null ? -1 : int.Parse(id);
        }

        public static string DisplayName(this ClaimsPrincipal principal)
        {
            var displayName = principal.FindFirst("name")?.Value ?? principal.FindFirst("http://schemas.microsoft.com/identity/claims/givenname")?.Value ?? principal.FindFirst(ClaimTypes.Name)?.Value;
            return displayName;
        }
    }
}
