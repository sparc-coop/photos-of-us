using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Kuvio.Kernel.Core.Services.Email;
using Kuvio.Kernel.Email.Sendgrid;
using Kuvio.Kernel.Database.SqlServer;
using Microsoft.EntityFrameworkCore;
using Kuvio.Kernel.Core;
using Kuvio.Kernel.Storage.Azure;
using PhotosOfUs.Model.Models;
using PhotosOfUs.Model;
using PhotosOfUs.WA.Server.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using PhotosOfUs.Connectors.Cognitive;
using PhotosOfUs.Core.Photos;
using PhotosOfUs.Core.Users.Commands;

namespace PhotosOfUs.WA.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            AddAuthentication(services);
            AddAuthorization(services);

            services.AddCors(options =>
            {
                options.AddPolicy("Client", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
            //services.AddServerSideBlazor();

            AddRepositories(services);
            AddServices(services);
            AddCommands(services);
            AddQueries(services);
            services.AddClaimsPrincipalInjector();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");

                //endpoints.MapDefaultControllerRoute();
            });
        }

        private void AddCommands(IServiceCollection services)
        {
            // Photos
            //services.AddTransient<...Command>();

            // Customers
        }

        private void AddServices(IServiceCollection services)
        {
            services.AddScoped(options => new SendGridService(Configuration));
            services.AddTransient<IEmailService, SendGridService>();

            //services.AddScoped(options => new PrismicService(Configuration));
            //services.AddTransient<PrismicService>();
        }

        private void AddQueries(IServiceCollection services)
        {
            // Photos
            //services.AddTransient<...Query>();


        }

        private void AddRepositories(IServiceCollection services)
        {
            services.AddDbContext<PhotosOfUsContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Database"]));
            services.AddScoped<DbContext, PhotosOfUsContext>();
            services.AddTransient(typeof(IRepository<>), typeof(DbRepository<>));
            //services.AddTransient(typeof(IDbRepository<>), typeof(DbRepository<>));
            //services.AddScoped<ICognitiveContext, AzureCognitiveContext>();

            services.AddScoped(options => new StorageContext(Configuration["ConnectionStrings:Storage"]));
            services.AddTransient<IMediaRepository, MediaRepository>();
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(AzureADB2CDefaults.AuthenticationScheme)
                .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));

            // To populate User.Identity.Name
            services.Configure<JwtBearerOptions>(
                AzureADB2CDefaults.JwtBearerAuthenticationScheme, options =>
                {
                    options.TokenValidationParameters.NameClaimType = "name";
                });

            //services.AddAuthentication(AzureADB2CDefaults.AuthenticationScheme)
            //    .AddAzureADB2C(options => Configuration.Bind("AzureAdB2C", options))
            //    .OnLogin(principal =>
            //    {
            //        services.BuildServiceProvider().GetRequiredService<LoginCommand>()
            //            .Execute(principal, principal.AzureID(), principal.Email(), principal.FirstName(), principal.LastName(), false);
            //    });

            //services.AddClaimsPrincipalInjector();

            services.AddControllersWithViews(options =>
            {
                // This enforces that users must be authenticated. 
                // If he isn't, he'll be redirected to the sign in page -- he won't even see the "Not Authorized" page
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("Client", builder => builder
            //        .AllowAnyOrigin()
            //        .AllowAnyHeader()
            //        .AllowAnyMethod()
            //        .AllowCredentials());
            //});

            //services.AddMvc(x => x.EnableEndpointRouting = false).SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);

            services.Configure<JwtBearerOptions>(AzureADB2CDefaults.JwtBearerAuthenticationScheme, options =>
            {
                options.TokenValidationParameters.NameClaimType = "name";
            });
        }

        private static void AddAuthorization(IServiceCollection services)
        {
            services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("User", policy =>
                {
                    policy.RequireRole("Admin", "User");
                });
            });
        }
    }
}
