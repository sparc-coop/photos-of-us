using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Kuvio.Kernel.AspNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PhotosOfUs.Model;
using PhotosOfUs.Model.Models;
using Microsoft.EntityFrameworkCore;
using PhotosOfUs.Connectors.Storage;
using PhotosOfUs.Connectors.Cognitive;
using Kuvio.Kernel.Core;
using PhotosOfUs.Connectors.Database;

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
            services.AddKuvioAuthentication(Configuration["AzureAdB2C:ClientId"], Configuration["AzureAdB2C:Tenant"], Configuration["AzureAdB2C:Policy"], OnLogin(services));
            services.AddDbContext<PhotosOfUsContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Database"]));
            services.AddScoped<DbContext, PhotosOfUsContext>();
            services.AddScoped(options => new StorageContext(Configuration["ConnectionStrings:Storage"]));
            services.AddScoped<ICognitiveContext, AzureCognitiveContext>();

            services.AddTransient(typeof(IRepository<>), typeof(DbRepository<>));
            services.AddTransient(typeof(IMediaRepository<>), typeof(MediaRepository<>));

            services.AddScoped<LoginCommand>();

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            app.UseRazorComponents<App.Startup>();
        }

        private static Action<System.Security.Claims.ClaimsPrincipal> OnLogin(IServiceCollection services)
        {
            return (principal) =>
            {
                services.BuildServiceProvider().GetRequiredService<LoginCommand>()
                    .Execute(principal, principal.AzureID(), principal.Email(), principal.DisplayName(), principal.HasClaim("tfp", "B2C_1_SiUpOrIn_Photographer"));
            };
        }
    }
}
