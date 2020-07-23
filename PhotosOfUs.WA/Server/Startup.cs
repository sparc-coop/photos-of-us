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

            services.AddControllersWithViews();
            services.AddRazorPages();

            AddRepositories(services);
            AddServices(services);
            AddCommands(services);
            AddQueries(services);
            //services.AddClaimsPrincipalInjector();
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

            //app.UseCors(policy =>
            //    policy.WithOrigins("https://localhost:44302", "https://localhost:5001")
            //    .AllowAnyMethod()
            //    .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, "x-custom-header")
            //    .AllowCredentials());
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


            services.AddDbContext<StockMeContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Database"]), ServiceLifetime.Transient);
            //services.AddDbContext<StockMeContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:Database"]));
            services.AddTransient(typeof(IRepository<>), typeof(DbRepository<>));
            //services.AddDbContext<StockMeContext>(options => options.UseInMemoryDatabase("MemoryDb"));
            //services.AddTransient(typeof(IRepository<>), typeof(InMemoryDbRepository<>));

            services.AddScoped(options => new StorageContext(Configuration["ConnectionStrings:Storage"]));
            services.AddTransient<IMediaRepository, MediaRepository>();
        }

        private void AddAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(AzureADB2CDefaults.AuthenticationScheme)
                            .AddAzureADB2C(options => Configuration.Bind("AzureAdB2C", options));

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
