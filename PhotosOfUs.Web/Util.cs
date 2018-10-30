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
            var id = principal.FindFirst(x => x.Type == "userid")?.Value;
            return id == null ? -1 : int.Parse(id);
        }


    }
}
