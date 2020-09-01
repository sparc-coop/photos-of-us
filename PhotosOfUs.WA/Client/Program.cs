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

            AddAuthentication(builder);
            AddAuthorization(builder);
            AddHttpClient(builder);

            builder.Services.AddBlazorModal();
            builder.Services.AddBlazorToast(options =>
            {
                options.Timeout = 5; // default: 5
                options.Position = Kuvio.Kernel.AspNet.Blazor.Toast.Configuration.ToastPosition.TopRight; // default: ToastPosition.TopRight
            });

            //JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            //{
            //    TypeNameHandling = TypeNameHandling.Objects
            //};

            await builder.Build().RunAsync();
        }

        private static void AddHttpClient(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddHttpClient("PhotosOfUs.ServerAPI", client => 
                            client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                            .CreateClient("PhotosOfUs.ServerAPI"));
        }

        private static void AddAuthorization(WebAssemblyHostBuilder builder)
        {
            

            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Photographer", policy => policy.RequireRole("Photographer"));
                options.AddPolicy("Customer", policy =>
                {
                    policy.RequireRole("Admin", "User");
                });
            });
        }

        private static void AddAuthentication(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add("https://photosofus1.onmicrosoft.com/e7d6ccf6-6f0d-4a63-ad73-d87bc15f7b68/API.Access");
            });
        }
    }
}
