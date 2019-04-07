using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Kuvio.Kernel.AspNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PhotosOfUs.Model;
using PhotosOfUs.Model.Models;
using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Connectors.Storage;
using PhotosOfUs.Connectors.Cognitive;
using Kuvio.Kernel.Core;
using PhotosOfUs.Connectors.Database;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication;

namespace PhotosOfUs.Client.Server
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorComponents<App.Startup>();

            services.AddAuthentication(o => o.DefaultAuthenticateScheme = AzureADB2CDefaults.CookieScheme)
                .AddAzureADB2C(options => Configuration.Bind("AzureAdB2C", options))
                .OnLogin(principal =>
                {
                    services.BuildServiceProvider().GetRequiredService<LoginCommand>()
                        .Execute(principal, principal.AzureID(), principal.Email(), principal.DisplayName(), principal.HasClaim("tfp", "B2C_1_SiUpOrIn_Photographer"));
                });


            services.AddDbContext<PhotosOfUsContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Database"]));
            services.AddScoped<DbContext, PhotosOfUsContext>();
            services.AddScoped(options => new StorageContext(Configuration["ConnectionStrings:Storage"]));
            services.AddScoped<ICognitiveContext, AzureCognitiveContext>();

            services.AddTransient(typeof(IRepository<>), typeof(DbRepository<>));
            services.AddTransient(typeof(IMediaRepository<>), typeof(MediaRepository<>));
            services.AddScoped<LoginCommand>();

            AddHttpClient(services);

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
        }

        private static void AddHttpClient(IServiceCollection services)
        {
            if (!services.Any(x => x.ServiceType == typeof(HttpClient)))
            {
                // Setup HttpClient for server side in a client side compatible fashion
                services.AddScoped(s =>
                {
                    // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
                    var uriHelper = s.GetRequiredService<IUriHelper>();
                    return new HttpClient
                    {
                        BaseAddress = new Uri(uriHelper.GetBaseUri())
                    };
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            app.UseRazorComponents<App.Startup>();
        }
    }
}
