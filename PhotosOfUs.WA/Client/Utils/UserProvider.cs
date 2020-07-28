using Kuvio.Kernel.Core;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;


namespace PhotosOfUs.WA.Client.Utils
{
    public class UserProvider
    {
        public UserProvider(AuthenticationStateProvider provider)
        {
            Provider = provider;
        }

        public async Task<ClaimsPrincipal> GetPrincipal() => (await Provider.GetAuthenticationStateAsync()).User;
        public AuthenticationStateProvider Provider { get; }
    }
}
