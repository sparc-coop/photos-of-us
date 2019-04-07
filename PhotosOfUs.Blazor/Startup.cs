using Kuvio.Kernel.AspNet;
using Kuvio.Kernel.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhotosOfUs.Blazor.Components;
using PhotosOfUs.Connectors.Cognitive;
using PhotosOfUs.Connectors.Database;
using PhotosOfUs.Connectors.Storage;
using PhotosOfUs.Model;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Blazor
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddNewtonsoftJson()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

            services.AddRazorComponents();

            // Authentication
            services.AddAuthentication(o => o.DefaultAuthenticateScheme = AzureADB2CDefaults.CookieScheme)
                .AddAzureADB2C(options => Configuration.Bind("AzureAdB2C", options))
                .OnLogin(principal =>
                {
                    services.BuildServiceProvider().GetRequiredService<LoginCommand>()
                        .Execute(principal, principal.AzureID(), principal.Email(), principal.DisplayName(), principal.HasClaim("tfp", "B2C_1_SiUpOrIn_Photographer"));
                });

            // Database
            services.AddDbContext<PhotosOfUsContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Database"]));
            services.AddScoped<DbContext, PhotosOfUsContext>();
            services.AddTransient(typeof(IRepository<>), typeof(DbRepository<>));

            // Storage
            services.AddScoped(options => new StorageContext(Configuration["ConnectionStrings:Storage"]));
            services.AddTransient(typeof(IMediaRepository<>), typeof(MediaRepository<>));

            // Other services
            services.AddScoped<ICognitiveContext, AzureCognitiveContext>();
            services.AddScoped<LoginCommand>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(routes =>
            {
                routes.MapRazorPages();
                routes.MapComponentHub<App>("app");
            });
        }

        public static string DefaultScriptPath { get; } = "_framework/components.server.js";
    }
}
