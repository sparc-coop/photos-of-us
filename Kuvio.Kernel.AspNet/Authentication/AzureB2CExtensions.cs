using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kuvio.Kernel.AspNet
{
    public static class AzureB2CExtensions
    {
        public static void AddKuvioAuthentication(this IServiceCollection services, string b2cClientId, string b2cTenant, string b2cPolicy)
        {
            services.AddAuthentication(o => o.DefaultAuthenticateScheme = AzureADB2CDefaults.CookieScheme)
                .AddAzureADB2C(options =>
                {
                    options.Instance = "https://login.microsoftonline.com/tfp/";
                    options.ClientId = b2cClientId;
                    options.CallbackPath = "/signin-oidc";
                    options.Domain = b2cTenant;
                    options.SignUpSignInPolicyId = b2cPolicy;
                });
        }

        public static void OnLogin(this AuthenticationBuilder builder, Action<ClaimsPrincipal> onLogin)
        { 
            builder.Services.Configure<OpenIdConnectOptions>(AzureADB2CDefaults.OpenIdScheme, options => {
                options.Events.OnTokenValidated = context => OnTokenValidatedAsync(context, onLogin);
            });

            builder.Services.AddClaimsPrincipalInjector();
        }

        public static void AddClaimsPrincipalInjector(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped(context =>
            {
                var httpContextAccessor = context.GetRequiredService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor?.HttpContext;
                return httpContext?.User;
            });
        }

        private static Task OnTokenValidatedAsync(Microsoft.AspNetCore.Authentication.OpenIdConnect.TokenValidatedContext context, Action<ClaimsPrincipal> onLogin)
        {
            onLogin(context.Principal);
            return Task.FromResult(0);
        }
    }
}
