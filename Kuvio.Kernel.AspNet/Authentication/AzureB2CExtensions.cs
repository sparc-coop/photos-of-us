using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kuvio.Kernel.AspNet
{
    public static class AzureB2CExtensions
    {
        public static void AddKuvioAuthentication(this IServiceCollection services, string b2cClientId, string b2cTenant, string b2cPolicy, Action<ClaimsPrincipal> onLogin, string jwtDomainName = null, string jwtSigningKey = null)
        {
            var builder = services.AddAuthentication();

            if (jwtDomainName != null && jwtSigningKey != null)
                builder.AddJwtBearer("Mobile", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtDomainName,
                        ValidAudience = jwtDomainName,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
                    };

                    // For SignalR mobile authentication -- websocket headers can't be messed with, so you have to pass the token in the query string
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Query.TryGetValue("accesstoken", out StringValues token))
                                context.Token = token;
                            return Task.FromResult(true);
                        }
                    };
                });

            builder.AddOpenIdConnect(options =>
                {
                    options.ClientId = b2cClientId;
                    options.Authority = $"https://login.microsoftonline.com/tfp/{b2cTenant}/{b2cPolicy}/v2.0/";
                    options.UseTokenLifetime = true;
                    options.SignInScheme = "B2C";
                    options.TokenValidationParameters = new TokenValidationParameters { NameClaimType = "name" };
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = (context) => OnRedirectToIdentityProviderAsync(context, b2cPolicy),
                        OnTokenValidated = (context) => OnTokenValidatedAsync(context, onLogin)
                    };
                })
                .AddCookie("B2C");

            services.AddAuthorization(options =>
            {
                if (jwtDomainName != null && jwtSigningKey != null)
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("B2C", "Mobile")
                        .Build();
                }
                else
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                       .RequireAuthenticatedUser()
                       .AddAuthenticationSchemes("B2C")
                       .Build();
                }
            });
        }


        private static Task OnTokenValidatedAsync(Microsoft.AspNetCore.Authentication.OpenIdConnect.TokenValidatedContext context, Action<ClaimsPrincipal> onLogin)
        {
            onLogin(context.Principal);
            return Task.FromResult(0);
        }

        private static Task OnRedirectToIdentityProviderAsync(RedirectContext context, string b2cPolicy)
        {
            var defaultPolicy = b2cPolicy;
            if (context.Properties.Items.TryGetValue("Policy", out var policy) && !policy.Equals(defaultPolicy))
            {
                context.ProtocolMessage.Scope = OpenIdConnectScope.OpenIdProfile;
                context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
                context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.ToLower().Replace(defaultPolicy.ToLower(), policy.ToLower());
                context.Properties.Items.Remove("Policy");
            }

            return Task.FromResult(0);
        }
    }
}
