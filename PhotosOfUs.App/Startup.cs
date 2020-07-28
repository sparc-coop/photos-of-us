using Kuvio.Kernel.Core;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhotosOfUs.Connectors.Cognitive;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Kuvio.Kernel.AspNet;

namespace PhotosOfUs.App
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            //// Authentication
            //services.AddAuthentication(o => o.DefaultAuthenticateScheme = AzureADB2CDefaults.CookieScheme)
            //    .AddAzureADB2C(options => Configuration.Bind("AzureAdB2C", options))
            //    .OnLogin(principal =>
            //    {
            //        services.BuildServiceProvider().GetRequiredService<LoginCommand>()
            //            .Execute(principal, principal.AzureID(), principal.Email(), principal.DisplayName(), principal.HasClaim("tfp", "B2C_1_SiUpOrIn_Photographer"));
            //    });

            //// Database
            //services.AddDbContext<PhotosOfUsContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Database"]));
            //services.AddScoped<DbContext, PhotosOfUsContext>();
            //services.AddTransient(typeof(IRepository<>), typeof(DbRepository<>));

            //// Storage
            //services.AddScoped(options => new StorageContext(Configuration["ConnectionStrings:Storage"]));
            //services.AddTransient(typeof(IMediaRepository<>), typeof(MediaRepository<>));

            //// Other services
            //services.AddScoped<ICognitiveContext, AzureCognitiveContext>();
            //services.AddScoped<LoginCommand>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
