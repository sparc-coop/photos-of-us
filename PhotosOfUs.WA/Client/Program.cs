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

            builder.Services.AddBlazorModal();
            builder.Services.AddBlazorToast(options =>
            {
                options.Timeout = 5; // default: 5
                options.Position = Kuvio.Kernel.AspNet.Blazor.Toast.Configuration.ToastPosition.TopRight; // default: ToastPosition.TopRight
            });

            await builder.Build().RunAsync();
        }

        private static void AddAuthorization(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("User", policy =>
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
            });
        }
    }
}
