using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Kuvio.Kernel.AspNet.Blazor;

using Kuvio.Kernel.Core;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using PhotosOfUs.WA.Client.Utils;
using Newtonsoft.Json;

namespace PhotosOfUs.WA.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            AddAuthentication(builder);
            AddAuthorization(builder);
            AddHttpClient(builder);

            builder.Services.AddBlazorModal();
            builder.Services.AddBlazorToast(options =>
            {
                options.Timeout = 5; // default: 5
                options.Position = Kuvio.Kernel.AspNet.Blazor.Toast.Configuration.ToastPosition.TopRight; // default: ToastPosition.TopRight
            });

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            await builder.Build().RunAsync();
        }

        private static void AddHttpClient(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddHttpClient("ServerAPI", client =>
                            client.BaseAddress = new Uri(builder.Configuration["ServerAPI:Url"]))
                            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("ServerAPI"));
        }

        private static void AddAuthorization(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddOptions();

            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("User", policy =>
                {
                    policy.RequireRole("Admin", "User");
                });
            });

            //builder.Services.AddTransient<ClaimsPrincipal>(async context => (await context.GetService<AuthenticationStateProvider>().GetAuthenticationStateAsync()).User ?? Task);
            //builder.Services.AddHttpContextAccessor();
            //builder.Services.AddScoped(context => context.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.User);
        }


        private static void AddAuthentication(WebAssemblyHostBuilder builder)
        {
            //builder.Services.AddMsalAuthentication(options =>
            //{
            //    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
            //    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://photosofus1.onmicrosoft.com/317b781b-53ca-4902-ab70-5d22db6e8f5d/API.Replication");
            //    options.UserOptions.NameClaim = "http://schemas.microsoft.com/identity/claims/objectidentifier";
            //});

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
            });

            builder.Services.AddTransient<UserProvider>();
        }
    }
}
